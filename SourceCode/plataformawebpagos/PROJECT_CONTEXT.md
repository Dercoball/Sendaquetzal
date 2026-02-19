# Project Context - plataformawebpagos

## Resumen ejecutivo
- Monolito ASP.NET WebForms (.NET Framework 4.8) llamado ApprestarSendaQuetzal.
- Dominio: gestion de prestamos, clientes/avales, pagos y cartera de inversionistas, con modulos de activos/materiales y reglas de comision.
- UI WebForms + JS (jQuery, DataTables, jqWidgets, TinyMCE) con llamadas AJAX a metodos `[WebMethod]` en code-behind.
- Acceso a datos via ADO.NET y Dapper contra SQL Server (cadena `connbd` en Web.config).
- Archivos y comprobantes se guardan en filesystem `/Uploads` y registros en DB; handler `FileUploader.ashx` crea thumbnails.
- PDF/Docs: Syncfusion DocIO/DocToPDF, iTextSharp, QRCoder; BouncyCastle/Pkcs para crypto; Twilio para SMS/WhatsApp; EASendMail para correo.
- Seguridad: autenticacion propia en `Login.aspx` (password MD5); autorizacion por `PermisoUsuario`/`Index.TienePermisoPagina` usando ids de pagina; session cookie `SENDAQUETZAL_SESSION`.

## Arquitectura y flujo
- Presentacion: paginas `.aspx` en `pages/*`, cada una con js en `js/app/**` y recursos en `vendor/css/img/fonts`.
- Logica de pagina: metodos estaticos `[WebMethod]` en code-behind (p.ej. `ReportDefault.GetTotals`) devuelven JSON; cada pagina define constante `pagina` para control de permisos.
- Modelos: POCOs en `Clases/` (Cliente, Prestamo, Pago, Inversion, Inversionista, Empleado, Plaza, Configuracion, Comision, MaterialEntrega, etc.), DTOs de Request/Response y enums.
- Datos: consultas SQL inline y algunos stored procedures via Dapper/SqlClient usando `ConnectionStrings[path]`; no capa DAL separada ni ORM.
- Archivos: `Uploads/Prestamos/{id}/{Cliente|Aval|...}` mas `pages/Uploads/*` usados por `FileUploader.ashx`.
- Reportes: `pages/Reports/ReportDefault.aspx` calcula totales/comisiones directamente con SQL; otros modulos generan PDF/Docx via plantillas (`plantillas/ticket_pago_01.docx`).
- Public site: `pages/Web` expone FAQ, terminos, tutoriales, about us y aviso de privacidad reutilizando los mismos datos.

## Mapa de modulos por carpeta
- `pages/Config`: catalogos base (usuarios, plazas, puestos/positions, categorias, calendarios/days_off, comisiones, mensajes Twilio, customer types, employees). 
- `pages/Customers`: alta/edicion de clientes y historico.
- `pages/Loans`: flujo de solicitud (`LoanRequest`), aprobacion (`LoanApprove`), pagos y moras (`Payments`/`PaymentsOverdue`), creditos extra; validaciones en `LoanValidation`; copias legacy `- Copia`.
- `pages/Investors`: inversionistas, inversiones, retiros y dashboard (`Utilities`, `Investments`, `Investors`).
- `pages/Assets`: activos y materiales, calendario de entregas.
- `pages/Commissions`: reglas y evaluacion de comisiones para empleados.
- `pages/Reports`: reportes de cartera/comisiones (`ReportDefault`).
- `pages/Web`: contenido publico (FAQ, terminos, tutoriales, about us, aviso de privacidad).
- `pages/Controles`: user controls `UcCliente` y `UcDocumentacion` usados en forms.
- `js/app`: scripts espejo de los modulos anteriores; `bundleconfig.json` define minificados; `vendor` aloja bootstrap/tinymce/datatables/jqwidgets/fullcalendar/toastr, etc.
- `flyway`: scripts SQL incrementales (V1..V7) y confs `main.conf`/`develop.conf`; usado por CircleCI job `flyway-migrate`.
- `Properties/PublishProfiles`: despliegues WebDeploy/FTP para varios targets `sendaquetzal*`.
- `.circleci/config.yml`: pipelines (branches develop/main) que hacen ssh a servidor ADCCOM, `git pull` y ejecutan `runAdcom.sh`, luego Flyway migrate.
- `Web.config`: pagina por defecto `pages/Home.aspx`, session InProc 30 min, max upload 1GB, MIME webp/svg, handler Syncfusion, conexion `connbd` a Azure SQL `sendaquetzalserver.database.windows.net`.

## Flujo general del sistema
1) Usuario ingresa en `Login.aspx`; credenciales se comparan via SQL (`usuario` tabla) con password MD5; se guarda `bitacora_login`; se cargan en Session: `path=connbd`, `usuario`, `id_usuario`, `id_empleado`, `id_tipo_usuario`.
2) Cada pagina en `Page_Load` verifica session y redirige a Login si falta; precarga campos ocultos (usuario, tipo, etc.).
3) Front-end JS invoca WebMethods con path/id_usuario; servidor valida permisos con `Index.TienePermisoPagina(pagina,id_usuario)` y ejecuta SQL (Dapper o SqlDataAdapter) sobre SQL Server; resultados se devuelven como listas/DTOs para DataTables o dashboards.
4) Operaciones de archivo llaman `FileUploader.ashx` que almacena file/thumbnail en filesystem y guarda base64 en DB segun tipo (documento cliente/empleado/garantia/comprobante inversion).
5) Reportes/descargas generan PDF/Docx y envios (Twilio/EASendMail) cuando aplica.

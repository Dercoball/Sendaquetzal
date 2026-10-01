# MANUAL DE USUARIO Y OPERACIONES
## Plataforma Web Senda Quetzal / Finaer
### Guía Práctica de Operación para Personal de Campo, Sucursal y Gerencia

---

## 1. Introducción y Acceso al Sistema

Este manual está diseñado exclusivamente para el **personal operativo** (Promotores, Supervisores, Ejecutivos, Gestores de Cobranza, Gerentes de Cobranza, Coordinadores, Directores de Plaza y Recursos Humanos). Su objetivo es explicar paso a paso cómo utilizar cada pantalla, pestaña, modal y botón del sistema en la operación diaria.

---

### 1.1 Ingreso a la Plataforma (`Login.aspx`)
1. Abra su navegador web (Google Chrome o Microsoft Edge recomendados).
2. Ingrese la dirección web oficial proporcionada por la empresa.
3. En la pantalla de bienvenida verá dos campos principales:
   - **Usuario / Login**: Escriba su identificador de usuario asignado.
   - **Contraseña**: Ingrese su contraseña secreta.
4. Presione el botón azul **Entrar**.
5. **Redirección automática**: Dependiendo de su puesto de trabajo, el sistema lo llevará directamente a su área de trabajo principal:
   - **Directores y Coordinadores**: Pantalla de Inicio / Dashboard general (`Index.aspx`).
   - **Supervisores, Ejecutivos y Promotores**: Bandeja de solicitudes de préstamo (`Loans/LoanRequest.aspx`).
   - **Gestores de Cobranza**: Cartera de cuentas vencidas (`Loans/PaymentsOverdue.aspx`).
   - **Gerente de Cobranza**: Cartera especializada de cobranza (`Cobranza/Cartera.aspx`).
   - **Recursos Humanos**: Directorio de colaboradores (`Config/Employees.aspx`).

> [!IMPORTANT]
> **Tiempo de Inactividad y Sesión Expirada**: Por seguridad de los datos de los clientes y del dinero de la empresa, el sistema se cierra automáticamente tras **30 minutos de inactividad**. Si el sistema lo regresa a la pantalla de inicio de sesión mientras capturaba datos, vuelva a ingresar sus credenciales; para evitar pérdida de información, guarde periódicamente sus avances.

---

### 1.2 Conociendo la Interfaz (`Index.aspx`)
Una vez dentro del sistema, encontrará tres áreas de control:

```
┌────────────────────────────────────────────────────────────────────────────────────────┐
│ [☰] Senda Quetzal / Finaer                                              [Usuario] [Salir]│
├───────────────────┬────────────────────────────────────────────────────────────────────┤
│ MENÚ PRINCIPAL    │ ÁREA DE TRABAJO                                                    │
│                   │                                                                    │
│ ⌂ Inicio          │ Aquí se despliegan las tablas, filtros, formularios y modales     │
│ 👤 Clientes       │ de la opción seleccionada en el menú lateral.                     │
│ 💳 Pagos          │                                                                    │
│ 💲 Préstamos      │                                                                    │
│ 💰 Cobranza       │                                                                    │
│ 📊 Inversionistas │                                                                    │
│ 📁 Otros / Activos│                                                                    │
│ ⚙ Configuración   │                                                                    │
└───────────────────┴────────────────────────────────────────────────────────────────────┘
```

1. **Barra Superior**:
   - Botón `☰` (Tres barras): Oculta o expande el menú lateral para tener más espacio en pantalla.
   - Nombre oficial de la empresa.
   - En el extremo derecho encontrará el botón **Salir** (icono de puerta de salida); úselo siempre antes de retirarse de su equipo.
2. **Menú Lateral Izquierdo**:
   - Contiene los módulos a los que su puesto tiene autorización.
   - En la parte superior del menú verá su fotografía o icono de usuario, su nombre completo y su puesto oficial.
3. **Área de Trabajo**:
   - Espacio central donde se muestran las tablas de búsqueda, tarjetas de captura, pestañas y ventanas modales de confirmación.

---

## 2. Guía Rápida por Rol Operativo (¿Qué me toca hacer?)

Antes de revisar las pantallas a detalle, consulte esta guía rápida para saber cuáles son sus responsabilidades dentro del sistema:

| Puesto / Rol | Actividades Clave en el Sistema | Pantallas que Utiliza |
| :--- | :--- | :--- |
| **Promotor** | Prospección de clientes, captura de solicitudes de préstamo, cobranza de cuotas corrientes en campo y entrega de recibos semanales. | `LoanRequest.aspx`, `LoanApprove.aspx`, `Payments.aspx`, `Customers.aspx`. |
| **Supervisor** | Visita ocular de domicilios, verificación de ubicación GPS, avalúo y fotografía de prendas en garantía, dictamen de aprobación técnica y supervisión de cobranza. | `LoanRequest.aspx`, `LoanApprove.aspx`, `Payments.aspx`, `PaymentsOverdue.aspx`, `ReportDefault.aspx`. |
| **Ejecutivo** | Visto bueno final de créditos aprobados por supervisores, análisis de capacidad de pago de plaza y autorización de aumento de créditos. | `LoanApprove.aspx`, `CreditIncreaseRequest.aspx`, `ReportDefault.aspx`. |
| **Gestor de Cobranza** | Recuperación en campo de cuentas difíciles (Semana 17 en adelante), aplicación de multas semanales ($50 c/u) y gasto de cobranza (10%), entrega de recibos provisionales foliados. | `PaymentsOverdue.aspx`, `Cartera.aspx`, `Reporte.aspx`. |
| **Gerente de Cobranza** | Asignación de cartera por buckets a gestores, emisión y entrega de blocks de folios, auditoría del cobro en campo y corte semanal de caja. | `Cartera.aspx`, `Folios.aspx`, `Reporte.aspx`. |
| **Coordinador / Director** | Consulta ejecutiva de colocación vs recuperación, arqueo de fondos de fin de semana, autorización de castigos de cartera (condonación) o pase a demanda legal. | `Index.aspx`, `Customers.aspx`, `ReportDefault.aspx`, `InvestmentsDashboard.aspx`. |
| **Recursos Humanos** | Altas de colaboradores nuevos, digitalización de expedientes y contratos laborales con sus avales de respaldo. | `Employees.aspx`, `NewEmployee.aspx`. |

---

## 3. Módulo de Clientes y Avales (Customers)

---

### 3.1 Padrón General de Clientes (`Customers.aspx`)
Esta pantalla permite consultar la totalidad de personas registradas en la financiera, verificar su historial de créditos y realizar acciones directas sobre su estatus.

```
┌────────────────────────────────────────────────────────────────────────────────────────────────┐
│ FILTROS: [Plaza: Todos ▼]  [Ejecutivo: Todos ▼]  [Supervisor: Todos ▼]  [Promotor: Todos ▼] [🔍 Filtrar] │
├────────────────────────────────────────────────────────────────────────────────────────────────┤
│ TABLA DE CLIENTES                                                                              │
│ No. │ Nombre    │ CURP       │ Teléfono   │ Monto (Min/Max) │ Dirección │ Estatus    │ Acciones│
│ 1   │ Juan Pérez│ PEZJ85...  │ 5512345678 │ $5,000.00       │ Calle 4.. │ Activo     │ [👁][⚙]│
└────────────────────────────────────────────────────────────────────────────────────────────────┘
```

#### Paso a Paso para Buscar un Cliente:
1. Diríjase en el menú a **Clientes**.
2. **Filtros por Equipo**:
   - `Plaza`: Seleccione la sucursal. *(Nota: Si usted es personal de plaza, su plaza estará fijada y bloqueada por seguridad).*
   - `Ejecutivo`, `Supervisor` y `Promotor`: Permiten filtrar la cartera por el empleado a cargo.
   - Presione el botón azul **Filtrar**.
3. **Filtros por Columna en la Tabla**:
   - Debajo del encabezado de cada columna encontrará cajas de texto para escribir el Nombre, la CURP, el Teléfono o el rango de Monto (mínimo y máximo).
   - En la columna **Estatus** puede filtrar directamente por:
     - `Activo`: Cliente con crédito vigente al corriente.
     - `Inactivo`: Cliente sin créditos activos o prospecto recién capturado.
     - `Vencido`: Cliente con semanas de atraso en cobranza.
     - `Condonado`: Cuenta con saldo perdonado o castigado por acuerdo de gerencia.
     - `Demanda`: Expediente remitido al área jurídica para cobranza judicial.

---

### 3.2 Modales y Acciones sobre Clientes

En la columna **Acciones** de la tabla de clientes encontrará los botones operativos:

#### 1. Ver Historial Crediticio (`CustomerHistory.aspx`):
- Al presionar el botón del **Ojo** o **Historial**, se abrirá la bitácora completa del cliente, mostrando:
  - Préstamos otorgados en el pasado.
  - Comportamiento de pago (puntualidad vs fallas).
  - Número de veces que ha fungido como aval de otros solicitantes.
  - Documentos e identificaciones archivadas.

#### 2. Modal Condonar Deuda (`#condonatePanel`):
- **¿Cuándo se usa?**: Cuando la Dirección General o el Comité de Crédito autoriza liquidar o perdonar el saldo insoluto de un cliente por fallecimiento, incobrabilidad extrema o negociación final.
- **¿Cómo operar?**:
  1. Haga clic en el botón **Condonar**.
  2. Aparecerá la ventana modal con el mensaje de advertencia: *"¿Está seguro de condonar el saldo de este cliente?"*.
  3. Presione **Aceptar** (`btnOkCondonate`).
  4. El crédito pasará a estatus `Condonado`, se congelarán los intereses y el cliente quedará registrado con dicho antecedente en su historial.

#### 3. Modal Enviar a Demanda Judicial (`#claimPanel`):
- **¿Cuándo se usa?**: Cuando se agotaron las vías de cobro extrajudicial y el área legal iniciará el juicio mercantil contra el cliente y sus avales utilizando los pagarés y prendas en garantía.
- **¿Cómo operar?**:
  1. Haga clic en el botón **Demanda**.
  2. Confirme la acción en el modal presionando **Aceptar** (`btnOkClaim`).
  3. El cliente cambiará su estatus a `Demanda` y se bloqueará para nuevas colocaciones en todo el sistema.

#### 4. Modal Reactivar Cliente (`#reactivatePanel`):
- **¿Cuándo se usa?**: Para rehabilitar a un cliente que estuvo suspendido, inactivo o en litigio una vez que saldó sus compromisos pendientes.
- **¿Cómo operar?**: Presione **Reactivar** y confirme en el modal presionando **Aceptar** (`btnOkReactivate`).

#### 5. Modal Eliminar Registro (`#panelEliminar`):
- Borra lógicamente el registro del cliente si fue capturado por duplicado o por error antes de tener créditos asociados.

---

## 4. Módulo de Solicitud y Aprobación de Préstamos (Loans)

---

### 4.1 Bandeja de Préstamos (`LoanRequest.aspx`)
Es el centro de control de colocación de crédito.

#### Botones Superiores:
- **NUEVO PRESTAMO** (Botón azul con signo de dinero): Abre la pantalla de captura y aprobación integral (`LoanApprove.aspx`).
- **BUSCAR**: Aplica los criterios de búsqueda de la tabla.
- **LIMPIAR**: Borra todos los filtros y muestra las solicitudes recientes.

#### Criterios de Búsqueda Avanzada:
- `Folio`: Número único de crédito.
- `No. préstamos máx. / mín.`: Filtra clientes según su nivel de renovación (ej. clientes con más de 3 préstamos).
- `Nombre`: Búsqueda por nombre o apellido.
- `Monto máx. / mín.`: Rango de dinero solicitado.
- `Fecha solicitud`: Rango de fechas de ingreso.
- `Promotor asignado`: Escriba el nombre del promotor que colocó la solicitud.
- `Aval máx. / mín.`: Número de veces que el solicitante ha sido aval de terceros.
- `Status`: Filtro por estatus de la solicitud (Solicitado, Aprobado, Rechazado, Pagado).

---

### 4.2 Captura y Aprobación Integral (`LoanApprove.aspx`)
Esta pantalla centraliza toda la originación del crédito a través de sus **5 Pestañas Obligatorias**.

```
┌────────────────────────────────────────────────────────────────────────────────────────┐
│ DATOS GENERALES DEL PRÉSTAMO:                                                          │
│ Fecha: [2026-09-08] Tipo Cliente: [Renovación ▼] Monto: [$6,000] Promotor: [Pedro ▼]   │
│ Veces como aval: [0]     Préstamos completados: [2]     Préstamos rechazados: [0]      │
├────────────────────────────────────────────────────────────────────────────────────────┤
│ [Pestaña 1: CLIENTE] [Pestaña 2: AVAL 1] [Pestaña 3: AVAL 2] [Pest 4: SUPERVISOR] [Pest 5: EJECUTIVO] │
├────────────────────────────────────────────────────────────────────────────────────────┤
│ CONTENIDO DE LA PESTAÑA SELECCIONADA...                                                │
│                                                                                        │
│                                                                                        │
├────────────────────────────────────────────────────────────────────────────────────────┤
│ ACCIONES: [REGRESAR]             [GUARDAR]        [RECHAZAR]        [APROBAR CRÉDITO] │
└────────────────────────────────────────────────────────────────────────────────────────┘
```

#### Paso a Paso para Capturar y Aprobar un Crédito:

#### PASO 1: Captura de Datos Generales (Tarjeta Superior)
1. **Fecha solicitud**: Ingrese la fecha del trámite.
2. **Tipo de cliente**: Seleccione si es *Nuevo*, *Renovación*, *Especial*, etc.
3. **Cantidad de préstamo**: Ingrese el monto en pesos a financiar.
4. **Máximo por renovación**: Si es renovación, valide el monto tope autorizado según el historial del cliente.
5. **Promotor**: Seleccione al promotor que atendió al solicitante.
6. Revise los campos informativos de apoyo: el sistema le indicará cuántos préstamos completados tiene y si actualmente es aval de alguien más.

---

#### PASO 2: Pestaña "Cliente" (`nav-client`)
En esta pestaña se capturan los datos generales y la documentación del solicitante:
1. **Formulario de Datos Personales**:
   - **CURP**: Ingrese la CURP oficial de 18 caracteres.
   - **Nombre(s)**, **Primer apellido** y **Segundo apellido**.
   - **Teléfono**: Número celular a 10 dígitos (es indispensable para el envío de recordatorios por SMS/WhatsApp).
   - **Ocupación**: Actividad laboral o giro del negocio.
   - **Dirección Particular**: Estado, Calle, Número, Colonia, Municipio y Código Postal.
   - **Ubicación GPS**: Presione el icono del **marcador satelital** (`fa-map-marker`) para obtener las coordenadas geográficas exactas de la vivienda.
   - **Dirección de trabajo**: Ubicación física de su empleo o comercio.
2. **Subida de Documentos (Lado Derecho)**:
   - Debe subir 4 archivos obligatorios en imagen (`.jpg`, `.png`) o documento.
   - Presione sobre el icono de la **Cámara** o la **Carpeta** en cada ranura:
     1. **Fotografía**: Foto clara del rostro del cliente.
     2. **Identificación frente**: Foto frontal de la credencial de elector (INE).
     3. **Identificación reverso**: Foto posterior del INE.
     4. **Comprobante domicilio**: Recibo reciente de luz, agua o teléfono.
   - Verifique que la imagen aparezca en la vista previa antes de avanzar.

---

#### PASO 3: Pestaña "Aval" (`nav-aval`)
Todo crédito requiere un obligado solidario.
1. Llene los mismos campos de datos personales, teléfono y dirección del **Aval 1**.
2. Suba sus 4 documentos oficiales (Foto, INE frente, INE reverso, Comprobante).

---

#### PASO 4: Pestaña "Aval 2" (`nav-aval2`)
- **Uso opcional**: Esta pestaña se utiliza cuando el monto solicitado es elevado o el cliente presenta capacidad de pago ajustada y requiere un segundo respaldo.
- Se capturan los datos y expedientes con la misma mecánica que el Aval 1. Si no se requiere segundo aval, puede dejarse vacía.

---

#### PASO 5: Pestaña "Aprobación Crédito" (Rol Supervisor) (`nav-aprobacion-supervisor`)
Esta pestaña es de uso exclusivo del **Supervisor de Campo**:
1. **Reconfirmar Ubicación**: El supervisor acude físicamente al domicilio del cliente y presiona el botón **Confirmar Ubicación** (`btnConfirmarUbicacion`). El sistema grabará el punto GPS de la inspección.
2. **Notas de Supervisión**: Redacte sus hallazgos de campo (ej. *"Se visitó tienda de abarrotes de la solicitante, cuenta con mercancía valuada en $30,000, comprobante coincide con domicilio y vecinos confirman arraigo de 5 años"*).
3. **Registro de Garantías Prendarias**:
   - Presione el botón azul **AGREGAR GARANTÍA** (`fa-plus-circle`).
   - Se abrirá la ventana modal **`#mdlGarantia`**:
     ```
     ┌─────────────────────────────────────────────────────────┐
     │ REGISTRO DE GARANTÍA PRENDARIA                          │
     ├─────────────────────────────────────────────────────────┤
     │ Nombre del Artículo: [Pantalla Samsung Smart TV 55"    ]│
     │ Número de Serie:     [SN-9876543210ABC                 ]│
     │ Costo / Avalúo ($):  [4,500.00                         ]│
     │ Fotografía del Bien: [Examinar...] (Subir foto del bien)│
     │                                                         │
     │         [CANCELAR]              [GUARDAR GARANTÍA]      │
     └─────────────────────────────────────────────────────────┘
     ```
   - Ingrese la descripción del bien empeñado, número de serie de fábrica, valor comercial o de remate estimado y suba la fotografía nítida del artículo.
   - Presione **GUARDAR**.
   - El bien aparecerá en la tabla `#tableGarantias` y el sistema sumará el total en pesos de las prendas empeñadas en la celda `#thMontoTotal`.

---

#### PASO 6: Pestaña "Aprobación Ejecutivo" (`nav-aprobacion-ejecutivo`)
- El **Ejecutivo de Plaza** revisa las notas del supervisor, el expediente y las garantías.
- Escribe su dictamen final en el campo `txtNotaAprobacionEjecutivo`.

---

#### PASO 7: Resolución Final del Crédito (Botonera Inferior)
- **GUARDAR**: Guarda el expediente como borrador para continuar la captura más tarde.
- **RECHAZAR**: Deniega la solicitud. El crédito pasa a estatus `Rechazado` y se notifica al promotor.
- **APROBAR CRÉDITO**: Autoriza formalmente la entrega del capital. El sistema genera el cronograma de cobro semanal, emite el número de crédito y habilita la impresión del pagaré y contrato oficial.

---

### 4.3 Solicitudes de Aumento de Crédito (`CreditIncreaseRequest.aspx`)
- Cuando un promotor intenta tramitar un crédito por un monto superior al permitido para ese tipo de cliente, el sistema genera una alerta automática.
- En esta pantalla, el **Ejecutivo** o **Director** revisa la lista de alertas:
  - Columnas: No. Alerta, Promotor, Supervisor, Límite Actual, Crédito Requerido, Plaza.
- Al hacer clic en la solicitud:
  - Presione **Aprobar** (Modal `#panelAprobar`) para autorizar el incremento extraordinario.
  - Presione **Rechazar** (Modal `#panelRechazar`) si el riesgo es alto, obligando al promotor a colocar únicamente el monto estándar.

---

## 5. Módulo de Cobranza Ordinaria y Cartera Semanal

---

### 5.1 Cobranza Semanal Corriente (`Payments.aspx`)
Es la herramienta de trabajo semanal de los promotores y supervisores para registrar las cuotas semanales de los créditos vigentes.

```
┌────────────────────────────────────────────────────────────────────────────────────────────────┐
│ COBRANZA SEMANAL - FILTROS: [Plaza ▼] [Ejecutivo ▼] [Supervisor ▼] [Promotor ▼] [🔍 Filtrar]   │
├────────────────────────────────────────────────────────────────────────────────────────────────┤
│ TABLA DE COBRANZA ORDINARIA                                                                    │
│ Sem. │ Cliente        │ Préstamo  │ F. Préstamo │ Último Pago │ Fallas │ Recibo │ Estatus │ Acc│
│ 4    │ María Gómez    │ $4,000.00 │ 2026-08-10  │ 2026-08-31  │ 0      │  [☑]   │ Aprobado│ [💳]│
└────────────────────────────────────────────────────────────────────────────────────────────────┘
```

#### Paso a Paso para Registrar un Abono Ordinario:
1. Ingrese a **Pagos**.
2. Filtre por su Plaza y su nombre de Promotor.
3. En la tabla se listarán todos los clientes que deben pagar en la semana en curso.
4. Identifique al cliente y haga clic en su fila para abrir el panel de cobro inferior (`#panelForm`):
   - **Cliente**: Nombre completo (solo lectura).
   - **Cantidad Prestada**: Importe total original del crédito.
   - **Saldo**: Cantidad exacta que aún debe el cliente.
   - **Abono**: Ingrese la cantidad en efectivo entregada por el cliente correspondiente a su semana (`txtAbono`).
   - **Recuperado**: Si el cliente además está cubriendo una cuota que debía de una semana pasada, capture ese importe en `txtRecuperado`.
5. Revise en la parte inferior la tabla **Historial**, donde se aprecian las fechas y montos de los abonos anteriores.
6. Presione el botón **Abonar / Guardar Pago**.
7. **Emisión de Recibo**: El sistema descontará el saldo en el acto y generará el recibo de pago con folio y código QR oficial (`ticket_pago_01.docx`), el cual puede imprimir o enviar por mensaje digital al cliente.

---

### 5.2 Cartera Vencida Tradicional (`PaymentsOverdue.aspx`)
Se utiliza para dar seguimiento a créditos que tienen **de 1 a 16 semanas con retraso** (antes de pasar a cobranza especializada).

#### Funciones de la Pantalla:
1. Muestra en la tabla el número de fallas acumuladas de cada cliente, el monto semanal acordado y el total exigible.
2. Al hacer clic sobre una cuenta en mora, se despliega el panel de cobranza con tres bloques de ayuda:
   - **Datos Cliente**: Calle, número exterior, teléfono directo y caja de texto para anotar el resultado de las llamadas (`txtNotasCliente`).
   - **Datos Aval**: Calle, teléfono del aval solidario y caja para registrar notas de visita al aval (`txtNotasAval`).
   - **Detalle de la Falla y Pago**: Número de semanas de atraso, monto vencido acumulado y caja para registrar el pago extraordinario (`txtMontoPago`).
3. Al capturar el pago y presionar **Guardar**, se habilita el botón amarillo **Recibo** (`btnRecibo`) para entregar el comprobante con sellos de cobranza.

---

## 6. Módulo de Cobranza Especializada (Semanas 17 en Adelante)

Este módulo rige bajo las políticas de recuperación judicial y extrajudicial (reglas V8 a V14). Entra en vigor cuando un crédito llega a la **Semana 17** sin haber sido finiquitado.

```
                  REGLAS DE ORO DE LA COBRANZA ESPECIALIZADA:
                  
  1. CAÍDA AUTOMÁTICA: Créditos con 17 o más semanas desde su inicio.
  2. MULTAS FIJAS: Se cobran $50.00 de multa por CADA semana con falla.
     (Atención: La "semana extra" TAMBIÉN genera los $50 de multa).
  3. GASTO DE COBRANZA: Se cobra el 10% sobre la suma de (Adeudo + Multas).
  4. COMISIONES DEL GESTOR (Porcentaje sobre lo que efectivamente cobre):
     • Bucket 1 (Semanas 17 a 19): 15% de comisión.
     • Bucket 2 (Semanas 20 a 23): 20% de comisión.
     • Bucket 3 (Semana 24 en adelante): 25% de comisión.
```

---

### 6.1 Gestión de Cartera Especializada (`Cartera.aspx`)
Es la pantalla de cabecera del **Gerente de Cobranza** y de los **Gestores de Cobranza**.

```
┌────────────────────────────────────────────────────────────────────────────────────────────────┐
│ CARTERA DE COBRANZA - FILTRO: [Plaza: Matriz ▼] [🔍 Filtrar]                                   │
├────────────────────────────────────────────────────────────────────────────────────────────────┤
│ TABLA DE CUENTAS EN MORA                                                                       │
│ Gestor  │ Cliente      │ Plaza  │ Sem. │ Bucket   │ Adeudo   │ Multas  │ Gasto 10%│ Total   │ Acc │
│ Ramirez │ Carlos Soto  │ Norte  │ 18   │ BUCKET 1 │ $3,200.00│ $150.00 │ $335.00  │ $3,685  │ [⚙] │
└────────────────────────────────────────────────────────────────────────────────────────────────┘
```

#### Paso a Paso para Operar una Cuenta en Cobranza:
1. Ingrese a **Cobranza > Cartera**.
2. Revise el semáforo de **Buckets** (etiquetas de color):
   - `Bucket 1` (Amarillo): Cuentas de 17 a 19 semanas.
   - `Bucket 2` (Rojo): Cuentas de 20 a 23 semanas.
   - `Bucket 3` (Vino/Café): Cuentas de 24 semanas o más (alto riesgo).
3. En la fila del cliente, presione el botón **Acción / Detalle**.
4. Se abrirá la ventana modal de auditoría **`#panelDetalle`**:

```
┌───────────────────────────────────────────────────────────────────────────────────────┐
│ DETALLE DE LA CUENTA                                                                  │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ CLIENTE: Carlos Soto       │ AVAL 1: Roberto Soto       │ AVAL 2: María López        │
│ Dir: Av. Hidalgo 123       │ Dir: Calle Juárez 45       │ Dir: Col. Centro 88        │
│ Tel: 5511223344            │ Tel: 5599887766            │ Tel: 5544332211            │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ DESGLOSE ECONÓMICO:                                                                   │
│ • Semanas transcurridas: 18 (Bucket 1)    • 14. Adeudo base:       $3,200.00          │
│ • Pago semanal pactado:  $250.00          • 15. Multas (3 fallas):   $150.00          │
│                                           • 16. Subtotal:          $3,350.00          │
│                                           • 17. Gasto Cobranza 10%:  $335.00          │
│                                           • 18. Total Exigible:    $3,685.00          │
│                                           • 19. Saldo Actualizado: $3,685.00          │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ ASIGNACIÓN DE GESTOR (Solo Gerencia):                                                 │
│ Gestor: [Juan Ramírez (Gestor 7) ▼] [Asignar gestor]                                  │
│ Observaciones confidenciales: [Acuerdo de pago para el viernes 12...] [Guardar obs]   │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ REGISTRAR COBRO:                                                                      │
│ Monto Recuperado: [$1,000.00] Canal: [Folio de cobranza ▼]                            │
│ Folio Provisional: [COB-1025] Fecha Cobro: [2026-09-08]                              │
│ Observación: [Pago parcial en efectivo en taller mecánico]                            │
│                                                              [REGISTRAR COBRO]        │
└───────────────────────────────────────────────────────────────────────────────────────┘
```

5. **Asignación (Exclusivo Gerencia)**:
   - El Gerente selecciona en el combo `cmbGestor` al gestor responsable y hace clic en **Asignar gestor**. La cuenta quedará registrada bajo su custodia.
6. **Captura de Cobro en Campo**:
   - Ingrese el dinero cobrado en **Monto recuperado**.
   - Seleccione el **Canal de pago**:
     - *Cobro en campo* (Efectivo entregado al gestor).
     - *Folio de cobranza* (Uso de recibo manual de block).
     - *Depósito a cuenta oficial* (Ficha bancaria de depósito).
     - *Formato de fallo* (Convenio de promesa de pago).
     - *Formato de recuperado* (Liquidación total convenida).
     - *Pago en oficina* (Cliente que acudió a sucursal).
   - Ingrese el número de **Folio** del recibo entregado al cliente.
   - Presione **Registrar cobro**. El sistema actualizará el saldo insoluto en tiempo real y acreditará la comisión correspondiente al gestor.

---

### 6.2 Control de Blocks y Recibos Provisionales (`Folios.aspx`)
Para evitar malos manejos de dinero en la calle, los gestores deben amparar todo cobro con un block de recibos foliados emitido por la gerencia.

#### Cómo Asignar un Block de Folios a un Gestor:
1. Ingrese a **Cobranza > Folios**.
2. En la tarjeta superior **Asignar block de folios**:
   - `Gestor`: Seleccione al gestor que recibirá el block.
   - `Prefijo`: Escriba el código del block (ej. `COB-`).
   - `Desde`: Folio inicial (ej. `501`).
   - `Hasta`: Folio final (ej. `550`).
   - Presione el botón azul **Asignar folios**.
3. El sistema creará los 50 folios individuales en estatus `Disponible`.
4. Cuando el gestor registre un cobro en `Cartera.aspx` utilizando el folio `COB-501`, este folio cambiará en automático su estatus a `Usado` registrando la fecha y el crédito donde se utilizó.

---

### 6.3 Corte Semanal y Liquidación de Gestores (`Reporte.aspx`)
Todos los lunes por la mañana se realiza el corte semanal de cobranza.

#### Cómo Interpretar el Reporte:
1. Ingrese a **Cobranza > Reporte semanal**.
2. Filtre por sucursal, seleccione al gestor y defina el rango de fechas (de lunes a domingo de la semana anterior).
3. **Tabla Superior (Detalle de Comisiones)**:
   - Muestra cada cobro efectuado por el gestor con el Bucket en el que se encontraba la cuenta y la comisión calculada en pesos (15%, 20% o 25%).
4. **Tabla Inferior (Resumen Económico de Caja)**:
   - Resuelve el cuadre financiero del gestor con 4 columnas fundamentales:
     - **Total Cobrado**: Todo el dinero que el gestor reportó haber recuperado.
     - **No Tangible**: Dinero que el gestor NO tiene en la mano (fichas de depósito bancario directo a cuenta de la empresa, pagos que el cliente hizo en ventanilla de oficina o convenios de fallo).
     - **Comisiones**: Total de comisiones ganadas por el gestor que la empresa debe pagarle.
     - **Diferencia (Efectivo a Entregar en Caja)**:
       $$\text{Diferencia} = \text{Total Cobrado} - \text{No Tangible} - \text{Comisiones}$$
     - Este importe final representa los billetes físicos exactos que el gestor debe depositar en la caja de la sucursal para liberar su corte semanal.

---

## 7. Módulo de Inversionistas y Rendimientos (Investors)

Este módulo gestiona los contratos con socios inversionistas que aportan capital para financiar la cartera de préstamos.

---

### 7.1 Directorio de Inversionistas (`Investors.aspx`)
1. Ingrese a **Inversionistas > Inversionistas**.
2. Para dar de alta a un nuevo socio inversionista, presione el botón **Nuevo**:
   - Se abrirá el modal `#panelEdicion`.
   - Ingrese: Nombre completo, RFC con homoclave, CURP, Teléfono, Correo Electrónico, Banco y Cuenta CLABE a 18 dígitos donde se le transferirán sus rendimientos.
   - Presione **Guardar**.
3. **Modales de Control**:
   - `#panelSuspender`: Pone en pausa a un inversionista para no renovar sus inversiones automáticamente.
   - `#panelEliminar`: Da de baja al inversionista si no cuenta con saldos activos.

---

### 7.2 Captura y Liquidación de Inversiones (`Investments.aspx`)

```
┌────────────────────────────────────────────────────────────────────────────────────────┐
│ INVERSIONES                                                                            │
├────────────────────────────────────────────────────────────────────────────────────────┤
│ [Pestaña: INVERTIR]                           [Pestaña: RETIRAR]                       │
├───────────────────────────────────────────────┬────────────────────────────────────────┤
│ Inversionista:     [Lic. Roberto Peña ▼]      │ Inversión:    [INV-2026-001 $100,000 ▼]│
│ Fecha Actual:      [2026-09-08]               │ Utilidad Acum:[$8,000.00]              │
│ Monto Inversión:   [$100,000.00]              │ Total Retiro: [$108,000.00]            │
│ % Rendimiento:     [8.0 %]                    │ Comprobante:  [Examinar... (SPEI)]     │
│ Utilidad en Pesos: [$8,000.00]                │                                        │
│ Plazo en días:     [90 días]                  │                                        │
│ Fecha Vencimiento: [2026-12-07]               │                                        │
│ Comprobante Dep:   [Examinar... (Ficha)]      │                                        │
│                                               │                                        │
│            [GUARDAR INVERSIÓN]                │            [GUARDAR RETIRO]            │
└───────────────────────────────────────────────┴────────────────────────────────────────┘
```

#### Flujo A: Registrar un Ingreso de Inversión (Pestaña "Invertir")
1. Ingrese a **Inversionistas > Inversiones** y abra la pestaña **Invertir** (`tabInvertir`).
2. Seleccione al Inversionista en el combo.
3. Ingrese el **Monto inversión** (ej. `$100,000.00`).
4. Ingrese el **% Utilidad** pactado (tasa de interés acordada).
5. Ingrese el **Plazo** en días naturales (ej. `30`, `60`, `90` o `180` días).
6. **Cálculo Automático**: El sistema calculará en el acto la **Utilidad en Pesos** (`txtUtilidadPesos`), la **Fecha de vencimiento/retiro** y la suma total acumulada (`txtUtilidadInversion`).
7. **Comprobante Bancario**: En el campo de archivo, suba la ficha o comprobante de transferencia bancaria por el cual ingresó el dinero a las cuentas de la financiera.
8. Presione **Guardar inversión**.

#### Flujo B: Liquidar o Retirar una Inversión (Pestaña "Retirar")
1. Abra la pestaña **Retirar** (`tabRetirar`).
2. Seleccione al Inversionista.
3. En el combo `cboFechaInversionesRetiro` seleccione el folio de la inversión vencida.
4. El sistema cargará el capital inicial, la utilidad devengada acumulada y el **Total a retirar** (`txtRetiro`).
5. Suba el comprobante de transferencia bancaria emitido hacia la cuenta CLABE del inversionista.
6. Presione **Guardar retiro**. La inversión quedará marcada como `Liquidada`.

---

### 7.3 Dashboard y Utilidades
- **`InvestmentsDashboard.aspx`**: Pantalla de consulta directiva que muestra mediante gráficos el volumen total de dinero fondeado por inversionistas versus el dinero que actualmente está colocado en las calles en microcréditos, vigilando que la empresa mantenga fondos de reserva suficientes.
- **`Utilities.aspx`**: Permite imprimir el estado de cuenta formal de un inversionista con sus fechas de abono, saldos y pagos de rendimientos históricos.

---

## 8. Módulo de Corte Semanal y Gastos (Reports & Bills)

---

### 8.1 Reporte Maestro de Fin de Semana (`ReportDefault.aspx`)
Es la pantalla de liquidación y cierre semanal de toda la sucursal (usada por Coordinadores, Supervisores y Auditores los viernes o sábados de corte).

#### Cómo Operar el Reporte de Determinación de Fondos:
1. Ingrese a **Reportes**.
2. Seleccione la **Semana** de corte en el selector de fecha y elija su **Plaza**.
3. Presione el botón **Reporte determinación** (`btnReporteDeterminacion`).
4. El sistema consolidará la información de campo y presentará cuatro tablas de cuadre:
   - **1. Tabla Principal de Promotoras (`tablePrincipal`)**:
     - Muestra por cada promotor: el dinero que *Debe entregar*, las *Fallas* (créditos no cobrados), el *Efectivo* físico recolectado, los abonos de *Semana extra*, los *Adelantos* de clientes, el `% de Falla` y las *Ventas* colocadas.
   - **2. Totales por Promotora (`tablePromotoraTotal`)**:
     - Arqueo neto de efectivo por cada cartera.
   - **3. Tabla de Gastos de Sucursal (`tableGastos`)**:
     - Egresos en gasolina, papelería, viáticos y servicios aprobados durante la semana.
   - **4. Concentrado General de Caja (`tableConcentrado`)**:
     - Muestra el resumen matemático de cierre:
       - Gastos autorizados.
       - Fondo fijo de caja disponible.
       - Monto total entregado por las promotoras.
       - **Gran Total Neto** que debe existir físicamente en la caja fuerte de la plaza.
5. **Firmas de Conformidad**:
   - Al pie del reporte se cuenta con los campos oficiales de rúbrica para el **Supervisor de Plaza**, el **Coordinador Regional** y el **Auditor Interno**.

---

### 8.2 Registro de Gastos de Sucursal (`Bills.aspx`)
1. Ingrese a **Gastos / Facturas**.
2. Para registrar un gasto operativo, presione el botón **Nuevo**:
   - Escriba el **Concepto** claro (ej. *"Gasolina ruta semanal supervisión norte"* o *"Pago de luz oficina local 3"*).
   - Ingrese la **Fecha** y el **Monto**.
   - Adjunte la fotografía del ticket o factura en PDF.
3. Presione **Guardar**. Este importe se descontará automáticamente en el corte semanal de `ReportDefault.aspx`.

---

## 9. Módulo de Recursos Humanos y Puestos (Config)

---

### 9.1 Alta de Colaboradores (`Employees.aspx` y `NewEmployee.aspx`)
Cada empleado de la financiera (Promotor, Supervisor, Gestor, Administrativo) debe contar con su expediente digitalizado en el sistema.

#### Paso a Paso para Registrar un Colaborador:
1. Ingrese a **Configuración > Colaboradores** y presione **Nuevo**. Se abrirá `NewEmployee.aspx`.
2. **Datos Laborales (Tarjeta Superior)**:
   - **Fecha ingreso**: Fecha de contratación formal.
   - **Puesto**: Seleccione la posición (Promotor, Supervisor, Gestor de Cobranza, Director, etc.).
   - **Plaza**: Sucursal de adscripción. *(Nota: Si el puesto es administrativo como Director, Capturista, Gerente de Cobranza o RH, no requiere plaza fija).*
   - **Supervisor y Ejecutivo**: Asigne a los mandos inmediatos responsables de este colaborador.
3. **Pestaña "Colaborador" (`nav-client`)**:
   - Llene los datos personales, CURP, teléfono y domicilio del colaborador.
   - Suba sus 4 documentos oficiales (Fotografía del empleado, INE frente, INE reverso y Comprobante de domicilio).
4. **Pestaña "Aval" (`nav-aval`)**:
   - Como política de seguridad interna, todo colaborador que maneja efectivo o valores debe registrar a un aval u obligado solidario.
   - Ingrese los datos y documentos digitalizados del aval del colaborador.
5. Presione el botón **GUARDAR**.

---

### 9.2 Cuentas de Acceso y Usuarios (`Usuarios.aspx`)
1. Una vez dado de alta el empleado, diríjase a **Configuración > Usuarios** para entregarle sus claves de acceso al sistema.
2. Presione **Nuevo**.
3. Ingrese el Nombre, su **Login** (identificador de acceso único, sin espacios) y su Correo electrónico.
4. Seleccione el **Tipo de Usuario** (debe coincidir con su puesto).
5. **Modal Cambio de Contraseña (`#panelEdicionPass`)**:
   - Si un colaborador olvidó su clave o debe asignársele una nueva, el administrador hace clic en el icono de la **Llave** en la tabla.
   - Escribe la nueva contraseña y presiona **Guardar**. La clave quedará encriptada y lista para su uso.

---

## 10. Módulo de Activos y Logística (Assets)

---

### 10.1 Inventario de Activos Fijos (`Assets.aspx`)
- Permite inventariar y resguardar bienes de la empresa (motocicletas de promotores, computadoras de oficina, impresoras, terminales).
- Para registrar un activo, presione **Agregar**:
  - Ingrese la **Categoría**, **Descripción del bien**, **Número de serie de fábrica**, **Costo de adquisición**, **Fecha de ingreso** y seleccione al **Colaborador / Plaza asignada**.
  - Si una motocicleta o equipo es dado de baja o vendido, se captura la **Fecha de baja** y el estatus cambia a `Inactivo`.

---

### 10.2 Entrega de Materiales y Calendario (`DeliveryMaterials.aspx` & `Calendar.aspx`)
- **`DeliveryMaterials.aspx`**: Registra la salida desde oficina matriz de paquetes de contratos membretados, pagarés oficiales pre-impresos, folletos y lonas hacia las distintas plazas. Permite conocer qué sucursal ha recibido material y cuántos formatos tiene disponibles.
- **`Calendar.aspx`**: Muestra en vista de calendario los días de entrega programados, los días festivos bancarios y los días de corte semanal para que ningún promotor planifique rutas en días no hábiles.

---

## 11. Solución de Problemas y Preguntas Frecuentes

### 1. "¿Por qué el sistema no me deja seleccionar otra plaza en los combos?"
- **Causa**: Por políticas de seguridad interna, los Promotores, Supervisores y Directores de plaza tienen restringida su visibilidad exclusivamente a su sucursal de adscripción para proteger la confidencialidad de los clientes. Solo la Dirección General, Coordinación y Gerencia de Cobranza pueden cambiar libremente de plaza.

### 2. "¿Por qué no me deja aprobar un crédito en `LoanApprove.aspx`?"
- **Causa**: El sistema cuenta con validaciones estrictas antes de liberar el dinero:
  1. Debe tener capturada la CURP válida y teléfonos.
  2. Deben estar cargadas las 4 imágenes de documentos tanto del cliente como del aval.
  3. El supervisor debe haber reconfirmado la ubicación GPS en la pestaña de supervisión.
  4. Si el monto excede el límite del tipo de cliente, debe existir una solicitud de aumento autorizada previamente en `CreditIncreaseRequest.aspx`.

### 3. "¿Qué hago si al capturar un cobro en cobranza me dice que el folio es inválido?"
- **Causa**: El número de folio provisional ingresado no pertenece al block que la gerencia le asignó a ese gestor en `Folios.aspx` o ya fue utilizado en otro crédito.
- **Solución**: El gestor debe verificar el número físico de su recibo y la gerencia debe confirmar que el block asignado en el sistema cubra esa numeración.

### 4. "¿Por qué me saca a la pantalla de login mientras estaba trabajando?"
- **Causa**: Su sesión permaneció sin interacción por más de 30 minutos.
- **Solución**: Vuelva a ingresar con su usuario y contraseña. Para evitar contratiempos en formularios largos (como la captura de clientes), presione periódicamente el botón **Guardar** para respaldar sus avances.

### 5. "¿Cómo se cancela o modifica un abono registrado por error?"
- **Causa**: Los registros de cobranza afectan los saldos contables y emiten tickets con código QR.
- **Solución**: Un promotor o gestor no puede borrar un pago unilateralmente. Debe notificar inmediatamente a la Dirección de Plaza o Gerencia de Cobranza con el número de folio y ticket para que el auditor realice la reversión autorizada en el sistema.

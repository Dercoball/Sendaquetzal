const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

console.log('Iniciando construcción del Manual de Operaciones en HTML y PDF...');

// Cargar imágenes corporativas a Base64 si existen
function getBase64Image(filePath) {
    try {
        if (fs.existsSync(filePath)) {
            const ext = path.extname(filePath).replace('.', '');
            const mime = ext === 'jpg' || ext === 'jpeg' ? 'image/jpeg' : ext === 'png' ? 'image/png' : 'image/svg+xml';
            const data = fs.readFileSync(filePath).toString('base64');
            return `data:${mime};base64,${data}`;
        }
    } catch (e) {
        console.error('Error leyendo imagen:', filePath, e.message);
    }
    return '';
}

const logoSq = getBase64Image('MyERP/img/sq.jpg');
const logoBrand = getBase64Image('MyERP/img/LogoIndex_1.png');

console.log('Logos cargados correctamente.');

// Generar el contenido HTML con diseño empresarial y mockups UI
const htmlContent = `<!DOCTYPE html>
<html lang="es">
<head>
<meta charset="UTF-8">
<title>Manual de Usuario y Operaciones - Senda Quetzal / Finaer</title>
<style>
  @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&family=JetBrains+Mono:wght@400;600&display=swap');

  @page {
    size: letter portrait;
    margin: 16mm 14mm 16mm 14mm;
    @bottom-right {
      content: counter(page);
    }
  }

  * {
    box-sizing: border-box;
    margin: 0;
    padding: 0;
  }

  body {
    font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
    color: #1E293B;
    background: #FFFFFF;
    font-size: 11pt;
    line-height: 1.55;
  }

  /* ESTILOS DE PORTADA */
  .cover-page {
    page-break-after: always;
    height: 100vh;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    padding: 40px 30px;
    background: linear-gradient(135deg, #022C22 0%, #064E3B 35%, #018F6D 100%);
    color: #FFFFFF;
    border-radius: 12px;
    position: relative;
    overflow: hidden;
  }

  .cover-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .cover-badge {
    background: rgba(255, 255, 255, 0.15);
    backdrop-filter: blur(8px);
    padding: 8px 18px;
    border-radius: 20px;
    font-size: 9.5pt;
    font-weight: 600;
    letter-spacing: 1px;
    text-transform: uppercase;
    border: 1px solid rgba(255, 255, 255, 0.25);
  }

  .cover-body {
    margin-top: 60px;
  }

  .cover-pretitle {
    color: #34D399;
    font-size: 13pt;
    font-weight: 700;
    letter-spacing: 2px;
    text-transform: uppercase;
    margin-bottom: 12px;
  }

  .cover-title {
    font-size: 32pt;
    font-weight: 800;
    line-height: 1.15;
    margin-bottom: 20px;
    color: #FFFFFF;
    text-shadow: 0 2px 10px rgba(0,0,0,0.2);
  }

  .cover-subtitle {
    font-size: 14pt;
    font-weight: 400;
    color: #E2E8F0;
    max-width: 650px;
    line-height: 1.5;
  }

  .cover-footer {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 20px;
    padding-top: 30px;
    border-top: 1px solid rgba(255, 255, 255, 0.2);
  }

  .cover-meta-item small {
    display: block;
    color: #A7F3D0;
    font-size: 8pt;
    text-transform: uppercase;
    letter-spacing: 1px;
    margin-bottom: 4px;
  }

  .cover-meta-item span {
    font-size: 10pt;
    font-weight: 600;
  }

  /* ESTRUCTURA DE PÁGINA */
  .page-header-bar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    border-bottom: 2px solid #018F6D;
    padding-bottom: 8px;
    margin-bottom: 20px;
    font-size: 9pt;
    color: #64748B;
    font-weight: 600;
  }

  .page-header-bar span.brand {
    color: #018F6D;
    font-weight: 800;
  }

  h1.chapter-title {
    font-size: 20pt;
    font-weight: 800;
    color: #0F172A;
    margin-bottom: 15px;
    padding-bottom: 8px;
    border-bottom: 1px solid #E2E8F0;
    display: flex;
    align-items: center;
    gap: 10px;
  }

  h1.chapter-title .num {
    background: #018F6D;
    color: #FFFFFF;
    width: 32px;
    height: 32px;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    border-radius: 8px;
    font-size: 14pt;
  }

  h2.section-title {
    font-size: 14pt;
    font-weight: 700;
    color: #064E3B;
    margin-top: 24px;
    margin-bottom: 12px;
    display: flex;
    align-items: center;
    gap: 8px;
  }

  h3.subsection-title {
    font-size: 11.5pt;
    font-weight: 600;
    color: #1E293B;
    margin-top: 16px;
    margin-bottom: 8px;
  }

  p {
    margin-bottom: 10px;
    color: #334155;
  }

  /* CAJAS DE ALERTA Y REGLAS */
  .alert-box {
    border-radius: 8px;
    padding: 12px 16px;
    margin: 14px 0;
    display: flex;
    gap: 12px;
    align-items: flex-start;
    font-size: 10pt;
    page-break-inside: avoid;
  }

  .alert-box.info {
    background: #EFF6FF;
    border-left: 4px solid #3B82F6;
    color: #1E40AF;
  }

  .alert-box.warning {
    background: #FFFBEB;
    border-left: 4px solid #F59E0B;
    color: #92400E;
  }

  .alert-box.danger {
    background: #FEF2F2;
    border-left: 4px solid #EF4444;
    color: #991B1B;
  }

  .alert-box.success {
    background: #ECFDF5;
    border-left: 4px solid #10B981;
    color: #065F46;
  }

  .alert-icon {
    font-size: 14pt;
    line-height: 1;
  }

  /* TABLAS PROFESIONALES */
  table.custom-table {
    width: 100%;
    border-collapse: collapse;
    margin: 14px 0;
    font-size: 9pt;
    page-break-inside: avoid;
  }

  table.custom-table th {
    background: #F1F5F9;
    color: #0F172A;
    font-weight: 700;
    text-align: left;
    padding: 8px 10px;
    border: 1px solid #CBD5E1;
  }

  table.custom-table td {
    padding: 7px 10px;
    border: 1px solid #E2E8F0;
    color: #334155;
  }

  table.custom-table tr:nth-child(even) td {
    background: #F8FAFC;
  }

  table.custom-table tfoot th {
    background: #E2E8F0;
    font-weight: 800;
  }

  /* BADGES */
  .badge {
    display: inline-block;
    padding: 3px 8px;
    border-radius: 12px;
    font-size: 8pt;
    font-weight: 700;
    text-transform: uppercase;
  }
  .badge.bucket-1 { background: #FEF3C7; color: #B45309; border: 1px solid #FCD34D; }
  .badge.bucket-2 { background: #FEE2E2; color: #B91C1C; border: 1px solid #FCA5A5; }
  .badge.bucket-3 { background: #7F1D1D; color: #FFFFFF; }
  .badge.activo { background: #D1FAE5; color: #065F46; }
  .badge.vencido { background: #FEE2E2; color: #991B1B; }
  .badge.condonado { background: #EDE9FE; color: #5B21B6; }
  .badge.demanda { background: #0F172A; color: #F8FAFC; }

  /* MOCKUPS DE PANTALLAS Y FORMULARIOS */
  .ui-mockup {
    background: #FFFFFF;
    border: 1px solid #CBD5E1;
    border-radius: 10px;
    box-shadow: 0 4px 12px rgba(0,0,0,0.06);
    margin: 16px 0 20px 0;
    overflow: hidden;
    page-break-inside: avoid;
  }

  .ui-mockup-header {
    background: #F8FAFC;
    border-bottom: 1px solid #E2E8F0;
    padding: 10px 14px;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .ui-mockup-title {
    font-size: 9.5pt;
    font-weight: 700;
    color: #0F172A;
    display: flex;
    align-items: center;
    gap: 8px;
  }

  .ui-mockup-dots {
    display: flex;
    gap: 5px;
  }

  .ui-mockup-dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
  }
  .ui-mockup-dot.red { background: #EF4444; }
  .ui-mockup-dot.yellow { background: #F59E0B; }
  .ui-mockup-dot.green { background: #10B981; }

  .ui-mockup-body {
    padding: 14px;
    background: #FFFFFF;
  }

  /* BOTONES UI */
  .btn-mock {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    padding: 6px 12px;
    border-radius: 6px;
    font-size: 8.5pt;
    font-weight: 600;
    cursor: default;
    border: none;
  }
  .btn-mock.primary { background: #018F6D; color: #FFF; }
  .btn-mock.secondary { background: #64748B; color: #FFF; }
  .btn-mock.danger { background: #DC2626; color: #FFF; }
  .btn-mock.warning { background: #D97706; color: #FFF; }
  .btn-mock.outline { background: transparent; border: 1px solid #CBD5E1; color: #334155; }

  /* CAMPOS DE FORMULARIO MOCK */
  .form-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
    gap: 10px;
    margin-bottom: 12px;
  }

  .form-group-mock {
    display: flex;
    flex-direction: column;
    gap: 3px;
  }

  .form-label-mock {
    font-size: 7.5pt;
    font-weight: 700;
    color: #475569;
    text-transform: uppercase;
    letter-spacing: 0.5px;
  }

  .form-control-mock {
    background: #F8FAFC;
    border: 1px solid #CBD5E1;
    border-radius: 5px;
    padding: 6px 8px;
    font-size: 8.5pt;
    color: #0F172A;
  }

  .form-control-mock.readonly {
    background: #F1F5F9;
    color: #64748B;
  }

  /* PESTAÑAS MOCK */
  .tabs-mock {
    display: flex;
    border-bottom: 2px solid #CBD5E1;
    margin-bottom: 12px;
    gap: 4px;
  }

  .tab-mock-item {
    padding: 7px 14px;
    font-size: 8.5pt;
    font-weight: 700;
    color: #64748B;
    background: #F1F5F9;
    border-radius: 6px 6px 0 0;
    border: 1px solid #CBD5E1;
    border-bottom: none;
  }

  .tab-mock-item.active {
    background: #018F6D;
    color: #FFFFFF;
    border-color: #018F6D;
  }

  /* MODAL MOCKUP */
  .modal-mockup {
    border: 2px solid #018F6D;
    border-radius: 10px;
    background: #FFFFFF;
    box-shadow: 0 10px 25px rgba(0,0,0,0.15);
    margin: 16px 0;
    overflow: hidden;
    page-break-inside: avoid;
  }

  .modal-mockup-header {
    background: #064E3B;
    color: #FFFFFF;
    padding: 10px 14px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-weight: 700;
    font-size: 9.5pt;
  }

  .modal-mockup-body {
    padding: 14px;
    background: #FFFFFF;
  }

  .modal-mockup-footer {
    background: #F8FAFC;
    border-top: 1px solid #E2E8F0;
    padding: 10px 14px;
    display: flex;
    justify-content: flex-end;
    gap: 8px;
  }

  /* FLUJOS EN DIAGRAMAS */
  .diagram-flow {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 8px;
    margin: 18px 0;
    page-break-inside: avoid;
  }

  .diagram-step {
    flex: 1;
    background: #F8FAFC;
    border: 1px solid #CBD5E1;
    border-top: 3px solid #018F6D;
    border-radius: 8px;
    padding: 10px 8px;
    text-align: center;
    font-size: 8pt;
  }

  .diagram-step strong {
    display: block;
    font-size: 8.5pt;
    color: #0F172A;
    margin-bottom: 4px;
  }

  .diagram-arrow {
    color: #018F6D;
    font-size: 14pt;
    font-weight: bold;
  }

  .page-break {
    page-break-after: always;
  }

  ul.step-list {
    margin-left: 18px;
    margin-bottom: 12px;
  }

  ul.step-list li {
    margin-bottom: 6px;
    color: #334155;
  }

  .doc-slot {
    border: 1px dashed #CBD5E1;
    border-radius: 6px;
    padding: 10px;
    text-align: center;
    background: #F8FAFC;
    font-size: 8pt;
  }
</style>
</head>
<body>

<!-- PORTADA DEL MANUAL -->
<div class="cover-page">
  <div class="cover-header">
    <div style="display:flex; align-items:center; gap:12px;">
      ${logoSq ? `<img src="${logoSq}" style="height:48px; border-radius:8px; background:#fff; padding:3px;">` : ''}
      <span style="font-size:16pt; font-weight:800; letter-spacing:1px;">SENDA QUETZAL</span>
    </div>
    <div class="cover-badge">Manual Operativo Oficial</div>
  </div>

  <div class="cover-body">
    <div class="cover-pretitle">Guía Integral de Usuario y Operaciones</div>
    <div class="cover-title">Manual de Uso Operativo del Sistema ERP</div>
    <div class="cover-subtitle">
      Plataforma Web para Administración de Micropréstamos, Inversiones Privadas, Expedientes Digitales y Cobranza Especializada en Campo.
    </div>
  </div>

  <div class="cover-footer">
    <div class="cover-meta-item">
      <small>Versión del Sistema</small>
      <span>V2.5 (Reglas V8–V14 Cobranza)</span>
    </div>
    <div class="cover-meta-item">
      <small>Audiencia y Distribución</small>
      <span>Promotores, Supervisores, Gestores, Gerencia</span>
    </div>
    <div class="cover-meta-item">
      <small>Vigencia y Actualización</small>
      <span>Año Operativo 2026</span>
    </div>
  </div>
</div>

<!-- CABECERA DE PÁGINA RECURRENTE -->
<div class="page-header-bar">
  <span><span class="brand">SENDA QUETZAL</span> | Manual de Operaciones de Sucursal y Campo</span>
  <span>Sistema ERP Web</span>
</div>

<!-- CAPÍTULO 1: ACCESO Y NAVEGACIÓN -->
<h1 class="chapter-title"><span class="num">1</span> Acceso, Seguridad y Navegación</h1>

<p>
  La plataforma <strong>Senda Quetzal / Finaer</strong> es el centro de operaciones donde se gestiona todo el ciclo financiero de la empresa: desde la solicitud de crédito de un cliente hasta el arqueo de caja de fin de semana y la liquidación de utilidades de los socios inversionistas.
</p>

<h2 class="section-title">1.1 Inicio de Sesión (Login.aspx)</h2>
<p>
  Para ingresar, abra Google Chrome o Microsoft Edge y digite la dirección web corporativa. Visualizará la pantalla de autenticación:
</p>

<div class="ui-mockup">
  <div class="ui-mockup-header">
    <div class="ui-mockup-title">🔐 Pantalla de Inicio de Sesión (Login.aspx)</div>
    <div class="ui-mockup-dots"><div class="ui-mockup-dot red"></div><div class="ui-mockup-dot yellow"></div><div class="ui-mockup-dot green"></div></div>
  </div>
  <div class="ui-mockup-body" style="display:flex; justify-content:center;">
    <div style="width:320px; padding:15px; border:1px solid #E2E8F0; border-radius:8px; background:#F8FAFC;">
      <div style="text-align:center; font-weight:800; font-size:14pt; color:#018F6D; margin-bottom:12px;">Senda Quetzal</div>
      <div class="form-group-mock" style="margin-bottom:10px;">
        <span class="form-label-mock">Usuario / Login</span>
        <div class="form-control-mock">juan.promotor</div>
      </div>
      <div class="form-group-mock" style="margin-bottom:15px;">
        <span class="form-label-mock">Contraseña</span>
        <div class="form-control-mock">••••••••••••</div>
      </div>
      <button class="btn-mock primary" style="width:100%; justify-content:center;">ENTRAR AL SISTEMA</button>
    </div>
  </div>
</div>

<div class="alert-box warning">
  <div class="alert-icon">⚠️</div>
  <div>
    <strong>Regla de Seguridad: Sesión Expirada por Inactividad (30 Minutos)</strong><br>
    Si deja la plataforma abierta sin realizar ninguna acción durante 30 minutos, el sistema cerrará la sesión de forma automática para evitar que personas no autorizadas manipulen saldos o expedientes. Al dar clic de nuevo, será redirigido a la pantalla de Login. Para no perder información en capturas largas, presione <strong>Guardar</strong> periódicamente.
  </div>
</div>

<h2 class="section-title">1.2 Redirección Automática por Puesto</h2>
<p>
  Al autenticarse exitosamente, el sistema no muestra la misma pantalla para todos. Cada colaborador es redirigido automáticamente a su área de trabajo diaria:
</p>
<ul class="step-list">
  <li><strong>Promotores, Supervisores y Ejecutivos</strong> ➔ Redirigidos a <code>Loans/LoanRequest.aspx</code> (Bandeja de Préstamos).</li>
  <li><strong>Gestores de Cobranza</strong> ➔ Redirigidos a <code>Loans/PaymentsOverdue.aspx</code> (Cuentas Vencidas).</li>
  <li><strong>Gerente de Cobranza</strong> ➔ Redirigido a <code>Cobranza/Cartera.aspx</code> (Cartera Especializada y Asignaciones).</li>
  <li><strong>Recursos Humanos</strong> ➔ Redirigido a <code>Config/Employees.aspx</code> (Directorio de Colaboradores).</li>
  <li><strong>Directores y Coordinadores</strong> ➔ Redirigidos a <code>Index.aspx</code> (Dashboard Principal).</li>
</ul>

<div class="page-break"></div>

<!-- CAPÍTULO 2: CLIENTES Y EXPEDIENTES -->
<div class="page-header-bar">
  <span><span class="brand">SENDA QUETZAL</span> | Módulo de Clientes y Avales</span>
  <span>Capítulo 2</span>
</div>

<h1 class="chapter-title"><span class="num">2</span> Padrón de Clientes y Acciones de Cartera</h1>

<p>
  La vista <code>pages/Customers/Customers.aspx</code> contiene la base de datos de todos los acreditados de la empresa. Permite filtrar por equipo comercial, revisar antecedentes y ejecutar acciones disciplinarias sobre la cartera.
</p>

<div class="ui-mockup">
  <div class="ui-mockup-header">
    <div class="ui-mockup-title">👥 Directorio de Clientes (Customers.aspx)</div>
    <div class="ui-mockup-dots"><div class="ui-mockup-dot red"></div><div class="ui-mockup-dot yellow"></div><div class="ui-mockup-dot green"></div></div>
  </div>
  <div class="ui-mockup-body">
    <div class="form-grid" style="grid-template-columns: repeat(4, 1fr); margin-bottom:12px;">
      <div class="form-group-mock"><span class="form-label-mock">Plaza</span><div class="form-control-mock">Plaza Norte (Fija)</div></div>
      <div class="form-group-mock"><span class="form-label-mock">Ejecutivo</span><div class="form-control-mock">Todos ▼</div></div>
      <div class="form-group-mock"><span class="form-label-mock">Supervisor</span><div class="form-control-mock">Carlos Vega ▼</div></div>
      <div class="form-group-mock"><span class="form-label-mock">Promotor</span><div class="form-control-mock">Pedro Silva ▼</div></div>
    </div>
    <table class="custom-table">
      <thead>
        <tr>
          <th>No.</th>
          <th>Nombre del Cliente</th>
          <th>CURP</th>
          <th>Teléfono</th>
          <th>Monto</th>
          <th>Estatus</th>
          <th>Acciones Operativas</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>001</td>
          <td><strong>María Elena Gómez</strong></td>
          <td>GOME820512MDF...</td>
          <td>55-1234-5678</td>
          <td>$5,000.00</td>
          <td><span class="badge activo">Activo</span></td>
          <td>
            <button class="btn-mock outline" style="padding:2px 6px; font-size:7.5pt;">👁 Historial</button>
            <button class="btn-mock danger" style="padding:2px 6px; font-size:7.5pt;">⚖ Demanda</button>
          </td>
        </tr>
        <tr>
          <td>002</td>
          <td><strong>Roberto Garza Peña</strong></td>
          <td>GAPR781103HNL...</td>
          <td>81-8899-0011</td>
          <td>$8,000.00</td>
          <td><span class="badge vencido">Vencido</span></td>
          <td>
            <button class="btn-mock warning" style="padding:2px 6px; font-size:7.5pt;">Condonar</button>
            <button class="btn-mock outline" style="padding:2px 6px; font-size:7.5pt;">Reactivar</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</div>

<h2 class="section-title">2.1 Los 4 Modales de Acción sobre Clientes</h2>

<div style="display:grid; grid-template-columns: 1fr 1fr; gap:14px; margin-top:10px;">
  <!-- MODAL 1: CONDONAR -->
  <div class="modal-mockup">
    <div class="modal-mockup-header">
      <span>Modal Condonar Saldo (#condonatePanel)</span>
    </div>
    <div class="modal-mockup-body">
      <p style="font-size:8.5pt;"><strong>¿Cuándo se usa?</strong> Cuando Dirección autoriza perdonar un saldo insoluto por fallecimiento, acuerdo judicial o castigo de cartera.</p>
      <div class="alert-box warning" style="margin:6px 0; padding:6px 10px; font-size:8pt;">
        Se cancelarán los intereses y el crédito pasará a estatus Condonado.
      </div>
    </div>
    <div class="modal-mockup-footer">
      <button class="btn-mock secondary" style="font-size:7.5pt;">Cancelar</button>
      <button class="btn-mock primary" style="font-size:7.5pt;">Aceptar Condonación</button>
    </div>
  </div>

  <!-- MODAL 2: DEMANDA -->
  <div class="modal-mockup">
    <div class="modal-mockup-header" style="background:#7F1D1D;">
      <span>Modal Demanda Legal (#claimPanel)</span>
    </div>
    <div class="modal-mockup-body">
      <p style="font-size:8.5pt;"><strong>¿Cuándo se usa?</strong> Cuando se agotaron todas las gestiones de cobranza amigable y se transfiere el expediente y pagarés al despacho jurídico.</p>
      <div class="alert-box danger" style="margin:6px 0; padding:6px 10px; font-size:8pt;">
        El cliente queda boletinado y bloqueado para nuevos créditos.
      </div>
    </div>
    <div class="modal-mockup-footer">
      <button class="btn-mock secondary" style="font-size:7.5pt;">Cancelar</button>
      <button class="btn-mock danger" style="font-size:7.5pt;">Confirmar Demanda</button>
    </div>
  </div>
</div>

<div style="display:grid; grid-template-columns: 1fr 1fr; gap:14px; margin-top:10px;">
  <!-- MODAL 3: REACTIVAR -->
  <div class="modal-mockup">
    <div class="modal-mockup-header" style="background:#0F766E;">
      <span>Modal Reactivar Cliente (#reactivatePanel)</span>
    </div>
    <div class="modal-mockup-body">
      <p style="font-size:8.5pt;"><strong>¿Cuándo se usa?</strong> Para rehabilitar a un cliente que saldó adeudos pendientes tras un castigo o demanda, permitiéndole solicitar préstamos nuevamente.</p>
    </div>
    <div class="modal-mockup-footer">
      <button class="btn-mock secondary" style="font-size:7.5pt;">Cancelar</button>
      <button class="btn-mock primary" style="font-size:7.5pt;">Reactivar Cliente</button>
    </div>
  </div>

  <!-- MODAL 4: HISTORIAL -->
  <div class="modal-mockup">
    <div class="modal-mockup-header" style="background:#1E293B;">
      <span>Expediente e Historial (CustomerHistory)</span>
    </div>
    <div class="modal-mockup-body">
      <p style="font-size:8.5pt;">Permite consultar préstamos anteriores, cuántas semanas pagó puntuales, cuántas fallas acumuló y si ha sido aval de otros clientes en la misma plaza.</p>
    </div>
    <div class="modal-mockup-footer">
      <button class="btn-mock outline" style="font-size:7.5pt;">Cerrar Historial</button>
    </div>
  </div>
</div>

<div class="page-break"></div>

<!-- CAPÍTULO 3: ORIGINACIÓN Y APROBACIÓN DE CRÉDITOS -->
<div class="page-header-bar">
  <span><span class="brand">SENDA QUETZAL</span> | Módulo de Solicitudes y Aprobaciones</span>
  <span>Capítulo 3</span>
</div>

<h1 class="chapter-title"><span class="num">3</span> Originación, Avales y Aprobación de Crédito</h1>

<h2 class="section-title">3.1 Flujo de Aprobación de Crédito (Paso a Paso)</h2>

<div class="diagram-flow">
  <div class="diagram-step">
    <strong>1. Prospección</strong>
    Captura de solicitud y datos generales
  </div>
  <div class="diagram-arrow">➔</div>
  <div class="diagram-step">
    <strong>2. Documentación</strong>
    4 Fotos obligatorias (Cliente y Aval)
  </div>
  <div class="diagram-arrow">➔</div>
  <div class="diagram-step">
    <strong>3. Supervisor</strong>
    GPS en campo y fotos de garantías
  </div>
  <div class="diagram-arrow">➔</div>
  <div class="diagram-step">
    <strong>4. Ejecutivo</strong>
    Visto bueno y dictamen de plaza
  </div>
  <div class="diagram-arrow">➔</div>
  <div class="diagram-step" style="border-top-color:#10B981; background:#ECFDF5;">
    <strong>5. Desembolso</strong>
    Aprobación, pagaré y cobro semanal
  </div>
</div>

<h2 class="section-title">3.2 Pantalla de Aprobación Multinivel (LoanApprove.aspx)</h2>
<p>
  Al presionar <strong>NUEVO PRESTAMO</strong> en <code>LoanRequest.aspx</code>, se abre el formulario integral de crédito compuesto por 5 pestañas:
</p>

<div class="ui-mockup">
  <div class="ui-mockup-header">
    <div class="ui-mockup-title">📑 Solicitud y Aprobación de Préstamo (LoanApprove.aspx)</div>
    <div class="ui-mockup-dots"><div class="ui-mockup-dot red"></div><div class="ui-mockup-dot yellow"></div><div class="ui-mockup-dot green"></div></div>
  </div>
  <div class="ui-mockup-body">
    <!-- CABECERA -->
    <div class="form-grid" style="grid-template-columns: repeat(4, 1fr); background:#F1F5F9; padding:10px; border-radius:6px; margin-bottom:12px;">
      <div class="form-group-mock"><span class="form-label-mock">Fecha Solicitud</span><div class="form-control-mock">2026-09-08</div></div>
      <div class="form-group-mock"><span class="form-label-mock">Tipo Cliente</span><div class="form-control-mock">Renovación ▼</div></div>
      <div class="form-group-mock"><span class="form-label-mock">Monto Crédito</span><div class="form-control-mock">$6,000.00</div></div>
      <div class="form-group-mock"><span class="form-label-mock">Promotor</span><div class="form-control-mock">Pedro Silva ▼</div></div>
    </div>

    <!-- PESTAÑAS -->
    <div class="tabs-mock">
      <div class="tab-mock-item active">1. Cliente</div>
      <div class="tab-mock-item">2. Aval 1</div>
      <div class="tab-mock-item">3. Aval 2 (Opcional)</div>
      <div class="tab-mock-item">4. Aprobación Crédito (Supervisor)</div>
      <div class="tab-mock-item">5. Aprobación Ejecutivo</div>
    </div>

    <!-- CONTENIDO PESTAÑA CLIENTE -->
    <div style="display:grid; grid-template-columns: 3fr 2fr; gap:12px;">
      <div>
        <div class="form-grid" style="grid-template-columns: 1fr 1fr;">
          <div class="form-group-mock"><span class="form-label-mock">CURP Oficial</span><div class="form-control-mock">PEZJ850614MDF...</div></div>
          <div class="form-group-mock"><span class="form-label-mock">Teléfono Celular</span><div class="form-control-mock">55-4433-2211</div></div>
        </div>
        <div class="form-grid" style="grid-template-columns: 1fr 1fr 1fr;">
          <div class="form-group-mock"><span class="form-label-mock">Nombre(s)</span><div class="form-control-mock">Juan Carlos</div></div>
          <div class="form-group-mock"><span class="form-label-mock">1er Apellido</span><div class="form-control-mock">Pérez</div></div>
          <div class="form-group-mock"><span class="form-label-mock">2do Apellido</span><div class="form-control-mock">Zavala</div></div>
        </div>
        <div class="form-group-mock" style="margin-bottom:8px;">
          <span class="form-label-mock">Dirección Particular</span>
          <div class="form-control-mock">Calle Reforma No. 402, Col. Centro, C.P. 64000</div>
        </div>
        <div class="form-group-mock">
          <span class="form-label-mock">Geolocalización Satelital (GPS)</span>
          <div style="display:flex; gap:6px;">
            <div class="form-control-mock" style="flex:1;">19.432608, -99.133209</div>
            <button class="btn-mock primary">📍 Fijar GPS</button>
          </div>
        </div>
      </div>

      <!-- RANURAS DE FOTOS -->
      <div style="display:grid; grid-template-columns: 1fr 1fr; gap:8px;">
        <div class="doc-slot">📷 <strong>Foto Personal</strong><br><small style="color:#059669;">[Cargada ✓]</small></div>
        <div class="doc-slot">🪪 <strong>INE Frente</strong><br><small style="color:#059669;">[Cargada ✓]</small></div>
        <div class="doc-slot">🪪 <strong>INE Reverso</strong><br><small style="color:#059669;">[Cargada ✓]</small></div>
        <div class="doc-slot">🏠 <strong>Comprobante Dom.</strong><br><small style="color:#059669;">[Cargada ✓]</small></div>
      </div>
    </div>

    <!-- BOTONERA DE ACCIÓN -->
    <div style="margin-top:16px; padding-top:12px; border-top:1px solid #E2E8F0; display:flex; justify-content:space-between; align-items:center;">
      <button class="btn-mock secondary">← REGRESAR</button>
      <div style="display:flex; gap:8px;">
        <button class="btn-mock primary">GUARDAR BORRADOR</button>
        <button class="btn-mock danger">RECHAZAR CRÉDITO</button>
        <button class="btn-mock" style="background:#059669; color:#fff; font-weight:800;">✓ APROBAR CRÉDITO</button>
      </div>
    </div>
  </div>
</div>

<h2 class="section-title">3.3 Pestaña Supervisor y Modal de Garantías (#mdlGarantia)</h2>
<p>
  En la <strong>Pestaña 4</strong>, el supervisor confirma las coordenadas GPS en el domicilio y captura las prendas en garantía prendaria presionando <strong>AGREGAR GARANTÍA</strong>:
</p>

<div class="modal-mockup" style="max-width:550px; margin: 10px auto;">
  <div class="modal-mockup-header">
    <span>Modal Registro de Garantía Prendaria (#mdlGarantia)</span>
  </div>
  <div class="modal-mockup-body">
    <div class="form-group-mock" style="margin-bottom:8px;">
      <span class="form-label-mock">Nombre y Descripción del Artículo</span>
      <div class="form-control-mock">Motocicleta Italika FT125 Roja Mod. 2023</div>
    </div>
    <div class="form-grid" style="grid-template-columns: 1fr 1fr;">
      <div class="form-group-mock">
        <span class="form-label-mock">Número de Serie / Motor</span>
        <div class="form-control-mock">3SCPE23B0P00192</div>
      </div>
      <div class="form-group-mock">
        <span class="form-label-mock">Valor Estimado de Avalúo ($)</span>
        <div class="form-control-mock">$8,500.00</div>
      </div>
    </div>
    <div class="form-group-mock" style="margin-top:8px;">
      <span class="form-label-mock">Fotografía del Artículo / Prenda</span>
      <div class="doc-slot" style="padding:15px; border-color:#018F6D;">
        📷 Seleccionar foto clara del vehículo o aparato electrodoméstico
      </div>
    </div>
  </div>
  <div class="modal-mockup-footer">
    <button class="btn-mock secondary">Cancelar</button>
    <button class="btn-mock primary">Guardar Garantía</button>
  </div>
</div>

<div class="page-break"></div>

<!-- CAPÍTULO 4: COBRANZA CORRIENTE Y RECIBOS -->
<div class="page-header-bar">
  <span><span class="brand">SENDA QUETZAL</span> | Cobranza Ordinaria y Recibos con QR</span>
  <span>Capítulo 4</span>
</div>

<h1 class="chapter-title"><span class="num">4</span> Cobranza Ordinaria Semanal</h1>

<p>
  En la pantalla <code>pages/Loans/Payments.aspx</code>, los promotores y supervisores registran el cobro de las cuotas semanales en campo y generan el comprobante formal con código QR.
</p>

<div class="ui-mockup">
  <div class="ui-mockup-header">
    <div class="ui-mockup-title">💳 Registro de Cobranza Semanal (Payments.aspx)</div>
    <div class="ui-mockup-dots"><div class="ui-mockup-dot red"></div><div class="ui-mockup-dot yellow"></div><div class="ui-mockup-dot green"></div></div>
  </div>
  <div class="ui-mockup-body">
    <table class="custom-table">
      <thead>
        <tr>
          <th>Sem.</th>
          <th>Nombre del Cliente</th>
          <th>Préstamo</th>
          <th>F. Préstamo</th>
          <th>Último Pago</th>
          <th>Fallas</th>
          <th>Estatus</th>
          <th>Acción</th>
        </tr>
      </thead>
      <tbody>
        <tr style="background:#ECFDF5;">
          <td><strong>Sem 3</strong></td>
          <td><strong>Juan Carlos Pérez</strong></td>
          <td>$6,000.00</td>
          <td>2026-08-15</td>
          <td>2026-08-29</td>
          <td>0</td>
          <td><span class="badge activo">Aprobado</span></td>
          <td><button class="btn-mock primary" style="padding:2px 8px;">Cobrar</button></td>
        </tr>
      </tbody>
    </table>

    <!-- FORMULARIO INFERIOR DE COBRO -->
    <div style="margin-top:14px; padding:12px; border:1px solid #CBD5E1; border-radius:8px; background:#F8FAFC;">
      <div style="font-weight:700; color:#064E3B; margin-bottom:10px; font-size:10pt;">Formulario de Aplicación de Abono</div>
      <div class="form-grid" style="grid-template-columns: repeat(4, 1fr);">
        <div class="form-group-mock"><span class="form-label-mock">Cliente</span><div class="form-control-mock readonly">Juan Carlos Pérez</div></div>
        <div class="form-group-mock"><span class="form-label-mock">Saldo Actual</span><div class="form-control-mock readonly">$4,500.00</div></div>
        <div class="form-group-mock"><span class="form-label-mock">Abono Semanal ($)</span><div class="form-control-mock" style="border-color:#018F6D; font-weight:700;">$500.00</div></div>
        <div class="form-group-mock"><span class="form-label-mock">Recuperado de Falla ($)</span><div class="form-control-mock">$0.00</div></div>
      </div>
      <div style="display:flex; justify-content:flex-end; gap:8px; margin-top:8px;">
        <button class="btn-mock secondary">Cancelar</button>
        <button class="btn-mock primary">✓ Guardar Abono</button>
        <button class="btn-mock warning">📄 Imprimir Ticket QR</button>
      </div>
    </div>
  </div>
</div>

<h2 class="section-title">4.1 Emisión y Validación del Recibo QR (ticket_pago_01.docx)</h2>
<p>
  Al guardar el abono, el sistema descuenta automáticamente el saldo y genera el ticket oficial de pago:
</p>
<ul class="step-list">
  <li><strong>Folio Consecutivo Único</strong>: Evita duplicidad de cobros.</li>
  <li><strong>Desglose del Saldo</strong>: Muestra cuánto abonó el cliente, cuánto debe aún y qué número de semana cubrió.</li>
  <li><strong>Código QR de Autenticidad</strong>: Permite al cliente o al auditor escanear con cualquier celular el código QR impreso para verificar en la base de datos que el dinero realmente ingresó a la empresa y no fue retenido por el cobrador.</li>
</ul>

<div class="alert-box info">
  <div class="alert-icon">💡</div>
  <div>
    <strong>Abono Ordinario vs. Abono Recuperado:</strong><br>
    - <strong>txtAbono (Abono ordinario)</strong>: Se usa para el pago de la cuota corriente que le correspondía pagar en la semana actual.<br>
    - <strong>txtRecuperado (Abono recuperado)</strong>: Se usa cuando el cliente tenía una falla de una semana anterior y hoy está entregando dinero adicional para ponerse al corriente. Este importe se clasifica por separado en el reporte de fin de semana para no distorsionar la comisión de cobranza.
  </div>
</div>

<div class="page-break"></div>

<!-- CAPÍTULO 5: COBRANZA ESPECIALIZADA -->
<div class="page-header-bar">
  <span><span class="brand">SENDA QUETZAL</span> | Cobranza Especializada (Semanas 17+)</span>
  <span>Capítulo 5</span>
</div>

<h1 class="chapter-title"><span class="num">5</span> Cobranza Especializada (Semanas 17+)</h1>

<p>
  El módulo de cobranza especializada rige bajo las políticas de recuperación judicial y extrajudicial (reglas V8 a V14). Entra en vigor cuando un crédito llega a la <strong>Semana 17</strong> sin haber sido liquidado.
</p>

<div class="alert-box danger">
  <div class="alert-icon">⚖️</div>
  <div>
    <strong>Reglas Oficiales de Penalización y Comisiones en Cobranza:</strong><br>
    1. <strong>Multas Fijas</strong>: Se cobran <strong>$50.00 pesos</strong> por CADA semana con falla sin pagar (incluyendo la semana extra).<br>
    2. <strong>Gasto de Cobranza</strong>: Se cobra el <strong>10%</strong> sobre la suma de <code>(Adeudo + Multas)</code>.<br>
    3. <strong>Comisiones del Gestor según Semáforo de Buckets</strong>:
    <div style="margin-top:6px; display:flex; gap:10px;">
      <span class="badge bucket-1">Bucket 1 (Sem 17 a 19): 15% Comisión</span>
      <span class="badge bucket-2">Bucket 2 (Sem 20 a 23): 20% Comisión</span>
      <span class="badge bucket-3">Bucket 3 (Sem 24 en adelante): 25% Comisión</span>
    </div>
  </div>
</div>

<h2 class="section-title">5.1 Cartera Especializada y Modal de Detalle (Cartera.aspx)</h2>

<div class="ui-mockup">
  <div class="ui-mockup-header">
    <div class="ui-mockup-title">💰 Cartera de Cobranza (Cartera.aspx)</div>
    <div class="ui-mockup-dots"><div class="ui-mockup-dot red"></div><div class="ui-mockup-dot yellow"></div><div class="ui-mockup-dot green"></div></div>
  </div>
  <div class="ui-mockup-body">
    <table class="custom-table">
      <thead>
        <tr>
          <th>Gestor</th>
          <th>Cliente</th>
          <th>Sem.</th>
          <th>Bucket</th>
          <th>Adeudo</th>
          <th>Multas</th>
          <th>Gasto 10%</th>
          <th>Total Exigible</th>
          <th>Acción</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>Ramírez</td>
          <td>Carlos Soto</td>
          <td>18</td>
          <td><span class="badge bucket-1">Bucket 1</span></td>
          <td>$3,200.00</td>
          <td>$150.00</td>
          <td>$335.00</td>
          <td><strong>$3,685.00</strong></td>
          <td><button class="btn-mock primary" style="padding:2px 6px;">Detalle</button></td>
        </tr>
      </tbody>
    </table>
  </div>
</div>

<div class="modal-mockup">
  <div class="modal-mockup-header">
    <span>Auditoría de Cuenta y Registro de Cobro (#panelDetalle)</span>
  </div>
  <div class="modal-mockup-body">
    <div style="display:grid; grid-template-columns: repeat(3, 1fr); gap:10px; border-bottom:1px solid #E2E8F0; padding-bottom:8px; margin-bottom:8px;">
      <div><small style="color:#64748B;">CLIENTE</small><br><strong>Carlos Soto</strong><br><small>Av. Hidalgo 123 (55-1122-3344)</small></div>
      <div><small style="color:#64748B;">AVAL 1</small><br><strong>Roberto Soto</strong><br><small>Calle Juárez 45 (55-9988-7766)</small></div>
      <div><small style="color:#64748B;">AVAL 2</small><br><strong>María López</strong><br><small>Col. Centro 88 (55-4433-2211)</small></div>
    </div>

    <!-- CÁLCULO ECONÓMICO -->
    <div style="display:grid; grid-template-columns: 1fr 1fr; gap:10px; background:#F8FAFC; padding:8px; border-radius:6px; margin-bottom:10px;">
      <div>
        <div><strong>Semanas transcurridas:</strong> 18 (Bucket 1)</div>
        <div><strong>14. Adeudo base:</strong> $3,200.00</div>
        <div><strong>15. Multas (3 fallas):</strong> $150.00</div>
      </div>
      <div>
        <div><strong>16. Subtotal:</strong> $3,350.00</div>
        <div><strong>17. Gasto Cobranza (10%):</strong> $335.00</div>
        <div style="font-size:11pt; color:#DC2626;"><strong>18. Total a Cobrar: $3,685.00</strong></div>
      </div>
    </div>

    <!-- CAPTURA DE COBRO -->
    <div style="border-top:1px dashed #CBD5E1; padding-top:8px;">
      <div style="font-weight:700; color:#064E3B; margin-bottom:6px;">Registrar Cobro en Campo</div>
      <div class="form-grid" style="grid-template-columns: repeat(4, 1fr);">
        <div class="form-group-mock"><span class="form-label-mock">Monto Recuperado ($)</span><div class="form-control-mock">$1,000.00</div></div>
        <div class="form-group-mock"><span class="form-label-mock">Canal de Pago</span><div class="form-control-mock">Folio de cobranza ▼</div></div>
        <div class="form-group-mock"><span class="form-label-mock">Folio de Recibo</span><div class="form-control-mock">COB-1025</div></div>
        <div class="form-group-mock"><span class="form-label-mock">Fecha</span><div class="form-control-mock">2026-09-08</div></div>
      </div>
    </div>
  </div>
  <div class="modal-mockup-footer">
    <button class="btn-mock secondary">Cerrar</button>
    <button class="btn-mock primary">✓ Registrar Cobro</button>
  </div>
</div>

<h2 class="section-title">5.2 Corte Semanal y Arqueo de Efectivo del Gestor (Reporte.aspx)</h2>
<p>
  El corte semanal determina cuánto dinero en efectivo debe entregar físicamente el gestor en caja:
</p>

<table class="custom-table">
  <thead>
    <tr>
      <th>Gestor Responsable</th>
      <th>Total Cobrado</th>
      <th>No Tangible (Bancos / Oficina)</th>
      <th>Comisiones Ganadas</th>
      <th>Diferencia en Efectivo (Caja)</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><strong>Juan Ramírez</strong></td>
      <td>$12,500.00</td>
      <td>$3,000.00</td>
      <td>$1,875.00</td>
      <td style="color:#065F46; font-weight:800; font-size:10pt;">$7,625.00</td>
    </tr>
  </tbody>
</table>

<div class="page-break"></div>

<!-- CAPÍTULO 6: INVERSIONISTAS Y FINANZAS -->
<div class="page-header-bar">
  <span><span class="brand">SENDA QUETZAL</span> | Fondeo de Inversionistas y Rendimientos</span>
  <span>Capítulo 6</span>
</div>

<h1 class="chapter-title"><span class="num">6</span> Socios Inversionistas y Rendimientos</h1>

<p>
  En el módulo <strong>Inversionistas</strong> (<code>Investors.aspx</code> e <code>Investments.aspx</code>), se gestiona el capital aportado por particulares para el fondeo de préstamos y el pago de sus rendimientos mensuales o trimestrales.
</p>

<h2 class="section-title">6.1 Captura de Inversión y Retiros (Investments.aspx)</h2>

<div class="ui-mockup">
  <div class="ui-mockup-header">
    <div class="ui-mockup-title">📈 Fondeo y Rendimientos (Investments.aspx)</div>
    <div class="ui-mockup-dots"><div class="ui-mockup-dot red"></div><div class="ui-mockup-dot yellow"></div><div class="ui-mockup-dot green"></div></div>
  </div>
  <div class="ui-mockup-body">
    <div class="tabs-mock">
      <div class="tab-mock-item active">Pestaña 1: Invertir Capital</div>
      <div class="tab-mock-item">Pestaña 2: Retirar Capital / Rendimientos</div>
    </div>

    <div class="form-grid" style="grid-template-columns: repeat(3, 1fr);">
      <div class="form-group-mock">
        <span class="form-label-mock">Inversionista Titular</span>
        <div class="form-control-mock">Lic. Roberto Peña Garza ▼</div>
      </div>
      <div class="form-group-mock">
        <span class="form-label-mock">Monto a Invertir ($)</span>
        <div class="form-control-mock" style="font-weight:700;">$100,000.00</div>
      </div>
      <div class="form-group-mock">
        <span class="form-label-mock">Tasa de Utilidad (%)</span>
        <div class="form-control-mock">8.0 % mensual</div>
      </div>
      <div class="form-group-mock">
        <span class="form-label-mock">Plazo en Días</span>
        <div class="form-control-mock">90 días naturales</div>
      </div>
      <div class="form-group-mock">
        <span class="form-label-mock">Rendimiento Proyectado ($)</span>
        <div class="form-control-mock readonly" style="color:#065F46; font-weight:700;">$8,000.00 pesos</div>
      </div>
      <div class="form-group-mock">
        <span class="form-label-mock">Fecha para Retiro</span>
        <div class="form-control-mock readonly">2026-12-07</div>
      </div>
    </div>

    <div class="form-group-mock" style="margin-top:10px;">
      <span class="form-label-mock">Comprobante Bancario de Depósito / Transferencia</span>
      <div class="doc-slot">
        📁 Subir comprobante SPEI o ficha de depósito que acredita el ingreso de los $100,000.00
      </div>
    </div>

    <div style="display:flex; justify-content:flex-end; gap:8px; margin-top:14px;">
      <button class="btn-mock secondary">Cancelar</button>
      <button class="btn-mock primary">✓ Guardar Inversión</button>
    </div>
  </div>
</div>

<h2 class="section-title">6.2 Supervisión de Liquidez (InvestmentsDashboard.aspx)</h2>
<p>
  El <strong>Director General</strong> consulta esta pantalla para verificar que el dinero de los inversionistas esté debidamente respaldado en cartera activa en la calle y que la financiera mantenga una reserva de liquidez prudencial para cumplir con retiros y contingencias operativas.
</p>

<div class="page-break"></div>

<!-- CAPÍTULO 7: CORTE SEMANAL Y RECURSOS HUMANOS -->
<div class="page-header-bar">
  <span><span class="brand">SENDA QUETZAL</span> | Determinación de Fondos y Colaboradores</span>
  <span>Capítulo 7</span>
</div>

<h1 class="chapter-title"><span class="num">7</span> Arqueo de Caja, Gastos y Colaboradores</h1>

<h2 class="section-title">7.1 Determinación de Fondos de Fin de Semana (ReportDefault.aspx)</h2>
<p>
  Los fines de semana, el Supervisor y el Coordinador generan el reporte de corte de sucursal:
</p>

<div class="ui-mockup">
  <div class="ui-mockup-header">
    <div class="ui-mockup-title">📑 Determinación de Fondos de Sucursal (ReportDefault.aspx)</div>
    <div class="ui-mockup-dots"><div class="ui-mockup-dot red"></div><div class="ui-mockup-dot yellow"></div><div class="ui-mockup-dot green"></div></div>
  </div>
  <div class="ui-mockup-body">
    <div style="display:grid; grid-template-columns: 2fr 1fr; gap:14px;">
      <div>
        <div style="font-weight:700; color:#064E3B; margin-bottom:6px; font-size:9pt;">1. Rendimiento por Promotora</div>
        <table class="custom-table" style="font-size:8pt;">
          <thead>
            <tr><th>Promotora</th><th>Debe Entregar</th><th>Fallas</th><th>Efectivo Recogido</th></tr>
          </thead>
          <tbody>
            <tr><td>Ruta Norte 1</td><td>$25,000.00</td><td>$2,500.00</td><td><strong>$22,500.00</strong></td></tr>
            <tr><td>Ruta Centro 2</td><td>$18,000.00</td><td>$1,200.00</td><td><strong>$16,800.00</strong></td></tr>
          </tbody>
        </table>
      </div>

      <div>
        <div style="font-weight:700; color:#064E3B; margin-bottom:6px; font-size:9pt;">2. Concentrado de Caja</div>
        <table class="custom-table" style="font-size:8pt;">
          <tbody>
            <tr><td>Gastos Autorizados</td><td style="color:#DC2626;">-$1,450.00</td></tr>
            <tr><td>Fondo Fijo Caja</td><td>+$5,000.00</td></tr>
            <tr><td>Cobranza Neta</td><td>+$39,300.00</td></tr>
            <tr style="background:#E2E8F0; font-weight:800;"><td>TOTAL EN CAJA</td><td style="color:#065F46;">$42,850.00</td></tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- BLOQUE DE FIRMAS -->
    <div style="display:grid; grid-template-columns: repeat(3, 1fr); gap:20px; margin-top:25px; text-align:center; font-size:8pt;">
      <div style="border-top:1px solid #000; padding-top:4px;">Supervisor de Plaza</div>
      <div style="border-top:1px solid #000; padding-top:4px;">Coordinador Regional</div>
      <div style="border-top:1px solid #000; padding-top:4px;">Auditor Interno</div>
    </div>
  </div>
</div>

<h2 class="section-title">7.2 Alta de Empleados con Aval Solidario (NewEmployee.aspx)</h2>
<p>
  En el departamento de <strong>Recursos Humanos</strong> (<code>Config/NewEmployee.aspx</code>), todo promotor o gestor que reciba dinero de cobranza debe ser registrado con su propio aval solidario:
</p>

<div class="tabs-mock">
  <div class="tab-mock-item active">Pestaña 1: Datos y Documentos del Colaborador</div>
  <div class="tab-mock-item">Pestaña 2: Datos y Documentos del Aval del Empleado</div>
</div>
<p style="font-size:9.5pt;">
  Ambas pestañas exigen subir los 4 documentos oficiales digitalizados (Fotografía, INE Frente, INE Reverso y Comprobante de Domicilio) para que el expediente laboral quede validado en el sistema.
</p>

<div class="page-break"></div>

<!-- CAPÍTULO 8: PREGUNTAS FRECUENTES Y SOLUCIÓN DE DUDAS -->
<div class="page-header-bar">
  <span><span class="brand">SENDA QUETZAL</span> | Preguntas Frecuentes y Solución de Problemas</span>
  <span>Capítulo 8</span>
</div>

<h1 class="chapter-title"><span class="num">8</span> Preguntas Frecuentes y Solución de Errores</h1>

<div style="margin-top:12px;">
  <h3 class="subsection-title">❓ 1. "¿Por qué el combo de Plaza aparece bloqueado y en gris?"</h3>
  <p>
    <strong>Explicación</strong>: Por estricta política de confidencialidad y control interno, los promotores, supervisores y directores de plaza tienen su combo fijado a su plaza asignada. Esto garantiza que nadie pueda ver ni alterar clientes de otra ciudad o sucursal. Solo Dirección General y Gerencia de Cobranza tienen acceso multiplaza.
  </p>

  <h3 class="subsection-title">❓ 2. "¿Por qué el botón 'APROBAR CRÉDITO' no se activa o marca error?"</h3>
  <p>
    <strong>Explicación</strong>: El sistema cuenta con validaciones candado obligatorias:
  </p>
  <ul class="step-list">
    <li>Faltan una o más de las 4 fotos obligatorias del Cliente o del Aval (Foto, INE frente, INE reverso o Comprobante).</li>
    <li>El supervisor no ha presionado el botón de confirmación de GPS en la Pestaña 4.</li>
    <li>El monto solicitado excede el tope permitido para ese cliente y no cuenta con autorización previa en <code>CreditIncreaseRequest.aspx</code>.</li>
  </ul>

  <h3 class="subsection-title">❓ 3. "¿Qué debo hacer si un folio de cobranza marca 'Folio Inválido o Usado'?"</h3>
  <p>
    <strong>Explicación</strong>: Ocurre cuando el gestor escribe un número de recibo que no está asignado a su nombre en <code>Folios.aspx</code> o que ya fue utilizado en otro crédito. El gestor debe revisar físicamente el block de papel y el Gerente de Cobranza debe verificar en el sistema qué rango de folios le fue otorgado.
  </p>

  <h3 class="subsection-title">❓ 4. "¿Cómo se cancela o modifica un pago que capturé por error?"</h3>
  <p>
    <strong>Explicación</strong>: Por seguridad financiera, ni los promotores ni los gestores pueden borrar cobros registrados, ya que estos emiten tickets oficiales con código QR. Debe avisar inmediatamente a su Director de Plaza o Gerencia de Cobranza con el número de folio y ticket para que el auditor realice la reversión autorizada en el sistema.
  </p>
</div>

<div class="alert-box success" style="margin-top:30px;">
  <div class="alert-icon">📘</div>
  <div>
    <strong>Fin del Manual Operativo</strong><br>
    Para cualquier duda sobre políticas de crédito o autorizaciones especiales, consulte con su Coordinador Regional o la Dirección General de Senda Quetzal / Finaer.
  </div>
</div>

</body>
</html>
`;

// Guardar archivo HTML enriquecido
const htmlPath = path.join(__dirname, 'manual_operaciones_senda_quetzal.html');
fs.writeFileSync(htmlPath, htmlContent, 'utf8');
console.log('Documento HTML generado en:', htmlPath);

// Ejecutar Microsoft Edge headless para imprimir a PDF
const pdfPath = path.join(__dirname, 'MANUAL_DE_OPERACIONES_SENDA_QUETZAL.pdf');
const edgePath = 'C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe';

console.log('Compilando a PDF mediante motor Chromium (Microsoft Edge headless)...');

try {
    const cmd = `"${edgePath}" --headless=new --disable-gpu --run-all-compositor-stages-before-draw --print-to-pdf="${pdfPath}" --no-pdf-header-footer "file:///${htmlPath.replace(/\\/g, '/')}"`;
    execSync(cmd);
    
    if (fs.existsSync(pdfPath)) {
        const stats = fs.statSync(pdfPath);
        console.log(`¡PDF generado exitosamente! Tamaño: ${(stats.size / 1024).toFixed(2)} KB en: ${pdfPath}`);
    } else {
        console.error('No se encontró el archivo PDF generado.');
    }
} catch (err) {
    console.error('Error al generar PDF con Edge:', err.message);
}

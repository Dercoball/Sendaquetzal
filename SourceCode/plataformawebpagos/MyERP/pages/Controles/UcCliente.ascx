<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UcCliente.ascx.cs" Inherits="Plataforma.pages.Controles.UcCliente" %>


<div class="row mb-3">
    <label class="form-label w-100">CURP</label>
    <input type="text" class="form-control campo-input" id="txtCURP" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Nombre(s)</label>
    <input type="text" class="form-control campo-input" id="txtNombre" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Primer apellido</label>
    <input type="text" class="form-control campo-input" id="txtPrimerApellido" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Segundo apellido</label>
    <input type="text" class="form-control campo-input" id="txtSegundoApellido" runat="server" />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Teléfono</label>
    <input type="text" class="form-control campo-input" id="txtTelefono" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Ocupación</label>
    <input type="text" class="form-control campo-input" id="txtOcupacion" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Estado</label>
    <input type="text" class="form-control campo-input" id="txtEstado" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Calle y número</label>
    <input type="text" class="form-control campo-input" id="txtCalle" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Colonia</label>
    <input type="text" class="form-control campo-input" id="txtColonia" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Municipio</label>
    <input type="text" class="form-control campo-input" id="txtMunicipio" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">C.P.</label>
    <input type="text" class="form-control campo-input" id="txtCodigoPostal" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Ubicación</label>
    <div class="input-group mb-3">
        <input type="text" class="form-control campo-input" id="txtUbicacion" runat="server"
            required="required" data-required-error='Requerido' />
        <div class="input-group-append">
            <button class="input-group-text" id="btnReloadLocation">
                <span class="fa fa-map-marker"></span>
            </button>
        </div>
        <div class="help-block with-errors"></div>
    </div>
</div>
<div class="row">
    <label class="form-label w-100">Dirección de trabajo</label>
    <input type="text" class="form-control campo-input" id="txtDireccionTrabajo" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>

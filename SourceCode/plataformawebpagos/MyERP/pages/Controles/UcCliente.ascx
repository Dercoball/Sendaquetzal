<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UcCliente.ascx.cs" Inherits="Plataforma.pages.Controles.UcCliente" %>


<div class="row mb-3">
    <label class="form-label w-100">CURP</label>
    <input type="text" class="form-control curp-persona" id="txtCURP" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Nombre(s)</label>
    <input type="text" class="form-control" id="txtNombre" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Primer apellido</label>
    <input type="text" class="form-control" id="txtPrimerApellido" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Segundo apellido</label>
    <input type="text" class="form-control" id="txtSegundoApellido" runat="server" />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Teléfono</label>
    <input type="text" class="form-control" id="txtTelefono" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Ocupación</label>
    <input type="text" class="form-control" id="txtOcupacion" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Estado</label>
    <input type="text" class="form-control" id="txtEstado" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Calle y número</label>
    <input type="text" class="form-control" id="txtCalle" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Colonia</label>
    <input type="text" class="form-control" id="txtColonia" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Municipio</label>
    <input type="text" class="form-control" id="txtMunicipio" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">C.P.</label>
    <input type="text" class="form-control" id="txtCodigoPostal" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>
<div class="row mb-3">
    <label class="form-label w-100">Ubicación</label>
    <div class="input-group mb-3">
        <input type="text" class="form-control" id="txtUbicacion" runat="server"
            required="required" data-required-error='Requerido' />
        <div class="input-group-append">
            <a class="input-group-text btnReloadLocation" id="btnReloadLocation" runat="server">
                <span class="fa fa-map-marker"></span>
            </a>
        </div>
        <div class="help-block with-errors"></div>
    </div>
</div>
<div class="row">
    <label class="form-label w-100">Dirección de trabajo</label>
    <input type="text" class="form-control" id="txtDireccionTrabajo" runat="server"
        required="required" data-required-error='Requerido' />
    <div class="help-block with-errors"></div>
</div>

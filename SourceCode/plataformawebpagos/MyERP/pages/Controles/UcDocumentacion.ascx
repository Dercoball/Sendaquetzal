<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UcDocumentacion.ascx.cs" Inherits="Plataforma.pages.Controles.UcDocumentacion" %>

<div class="row col-12  mb-3">
    <label class="form-label w-100">
        <input type="file" id="filefotografia" runat="server" class="fileImagenes" />Fotografía <i class="ml-4 fa fa-camera"></i>
    </label>
    <input type="hidden" id="hfIdDocumento1" value="0" runat="server"/>
    <img id="imgDocumento1"  runat="server" class="img-fluid rounded" style="max-height: 280px; width: 100%;" />
</div>
<div class="row  col-12 mb-3">
    <label class="form-label w-100">
        <input type="file"  id="fileidentificacionfrente" class="fileImagenes" runat="server"  />Identificación frente <i class="ml-4 fa fa-folder"></i>
    </label>
    <input type="hidden" id="hfIdDocumento2" value="0" runat="server"/>
    <img id="imgDocumento2" runat="server"  class="img-fluid rounded" style="max-height: 280px; width: 100%;" />
</div>
<div class="row  col-12 mb-3">
    <label class="form-label w-100">
        <input type="file" id="fileidentificacionreverso" class="fileImagenes" runat="server" />Identificación reverso <i class="ml-4 fa fa-folder"></i>
    </label>
    <input type="hidden" id="hfIdDocumento3" value="0" runat="server"/>
    <img id="imgDocumento3" runat="server"  class="img-fluid rounded" style="max-height: 280px; width: 100%;" />
</div>
<div class="row  col-12 mb-3">
    <label class="form-label w-100">
        <input type="hidden" id="hfIdDocumento4" value="0" runat="server"/>
        <input type="file"  id="filecomprobantedomicilio" class="fileImagenes" runat="server" />Comprobante domicilio <i class="ml-4 fa fa-folder"></i>
    </label>
    <img id="imgDocumento4" runat="server"  class="img-fluid rounded" style="max-height: 280px; width: 100%;" />
</div>

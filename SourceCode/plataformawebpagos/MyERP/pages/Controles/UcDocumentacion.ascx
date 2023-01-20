<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UcDocumentacion.ascx.cs" Inherits="Plataforma.pages.Controles.UcDocumentacion" %>

<div class="row col-12  mb-3">
    <label class="form-label w-100">
        <input type="file" id="filefotografia" runat="server" />Fotografía <i class="ml-4 fa fa-camera"></i>
    </label>
    <img id="imgFotografia"  runat="server" class="img-fluid rounded" style="max-height: 280px; width: 100%;" />
</div>
<div class="row  col-12 mb-3">
    <label class="form-label w-100">
        <input type="file"  id="fileidentificacionfrente" runat="server"  />Identificación frente <i class="ml-4 fa fa-folder"></i>
    </label>
    <img id="imgFrente" runat="server"  class="img-fluid rounded" style="max-height: 280px; width: 100%;" />
</div>
<div class="row  col-12 mb-3">
    <label class="form-label w-100">
        <input type="file" id="fileidentificacionreverso" runat="server" />Identificación reverso <i class="ml-4 fa fa-folder"></i>
    </label>
    <img id="imgReverso" runat="server"  class="img-fluid rounded" style="max-height: 280px; width: 100%;" />
</div>
<div class="row  col-12 mb-3">
    <label class="form-label w-100">
        <input type="file"  id="filecomprobantedomicilio" runat="server" />Comprobante domicilio <i class="ml-4 fa fa-folder"></i>
    </label>
    <img id="imgDomicilio" runat="server"  class="img-fluid rounded" style="max-height: 280px; width: 100%;" />
</div>

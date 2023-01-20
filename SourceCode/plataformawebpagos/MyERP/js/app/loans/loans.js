'use strict';
let date = new Date();
let descargas = "Prestamo_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '13';


const loans = {
    init: () => {
        loans.idPrestamo = -1;
        loans.idAval = -1;
        loans.idCliente = -1;
        loans.idDireccionCliente = -1;
        loans.idDireccionAval = -1;
        loans.arrDocumentosCliente = {};
        loans.arrDocumentosAval = {};

        $("#lblFechaSolicitud").html(moment(Date.now()).format('YYYY-MM-DD'))
        loans.loadTipoClientes();
    },
    loadTipoClientes: () => {
        var params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $('#cboTipoCliente').html('');

        $.ajax({
            type: "POST",
            url: "/pages/Config/CustomerTypes.aspx/GetListaItems",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var llst_TipoClientes = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < llst_TipoClientes.length; i++) {
                    let lo_TipoCliente = llst_TipoClientes[i];
                    opcion += `<option value = '${lo_TipoCliente.IdTipoCliente}' > ${lo_TipoCliente.NombreTipoCliente}</option > `;
                }

                $('#cboTipoCliente').html(opcion);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    loadPreviewImg: (upload,  funcion) => {
        if (upload.files && upload.files[0]) {
            var reader = new FileReader();
            reader.onload = funcion;
            //reader.onload = function (e) {
            //    $('#' + img).attr('src', e.target.result);
            //}
            reader.readAsDataURL(upload.files[0]);
        } else {
            utils.toast('Debe cargar la imagen', 'info')
        }
    },
    setDireccion: (oDireccion, control) => {
        $("#" + control + "_txtEstado").val(oDireccion.Estado);
        $("#" + control + "_txtCalle").val(oDireccion.Calle);
        $("#" + control + "_txtColonia").val(oDireccion.Colonia);
        $("#" + control + "_txtMunicipio").val(oDireccion.Municipio);
        $("#" + control + "_txtCodigoPostal").val(oDireccion.CodigoPostal);
        $("#" + control + "_txtUbicacion").val(oDireccion.Ubicacion);
        $("#" + control + "_txtDireccionTrabajo").val(oDireccion.DireccionTrabajo);
    },
    getDireccion: (control) => {
        var oDireccion = {};

        oDireccion.Calle = $("#" + control + "_txtCalle").val();
        oDireccion.Colonia = $("#" + control + "_txtColonia").val();
        oDireccion.Municipio = $("#" + control + "_txtMunicipio").val();
        oDireccion.Estado = $("#" + control + "_txtEstado").val();
        oDireccion.CodigoPostal = $("#" + control + "_txtCodigoPostal").val();
        oDireccion.DireccionTrabajo = $("#" + control + "_txtDireccionTrabajo").val();
        oDireccion.Ubicacion = $("#" + control + "_txtUbicacion").val();

        return oDireccion;
    },
    getPrestamo: () => {
        var oPrestamo = {};

        oPrestamo.IdPrestamo = loans.idPrestamo;
        oPrestamo.FechaSolicitud = moment($("#lblFechaSolicitud").html()).format('YYYY-MM-DD');
        oPrestamo.IdTipoCliente = $("#cboTipoCliente").val()
        oPrestamo.Monto = $("#txtCantidadPrestamo").val()

        return oPrestamo;
    },
    getDatosPersona: (control) => {
        var oDatosPersona = {};
        
        oDatosPersona.Curp = $("#" + control + "_txtCURP").val();
        oDatosPersona.Nombre = $("#" + control + "_txtNombre").val();
        oDatosPersona.PrimerApellido = $("#" + control + "_txtPrimerApellido").val();
        oDatosPersona.SegundoApellido = $("#" + control + "_txtSegundoApellido").val();
        oDatosPersona.Telefono = $("#" + control + "_txtTelefono").val();
        oDatosPersona.Ocupacion = $("#" + control + "_txtOcupacion").val();
        oDatosPersona.direccion = loans.getDireccion(control);

        return oDatosPersona;
    },
    getDocumentos: (control) => {
        var arrDocumentos = [];
        var filefotografia = $("#" + control +"_filefotografia");
        var fileidentificacionfrente = $("#" + control +"_fileidentificacionfrente");
        var fileidentificacionreverso = $("#" + control +"_fileidentificacionreverso");
        var filecomprobantedomicilio = $("#" + control +"_filecomprobantedomicilio");

        var imgFotografia = $("#" + control + "_imgFotografia");
        var imgFrente = $("#" + control + "_imgFrente");
        var imgReverso = $("#" + control + "_imgReverso");
        var imgDomicilio = $("#" + control + "_imgDomicilio");

        if (filefotografia[0].files.length > 0) {
            arrDocumentos.push({
                IdTipoDocumento: 1,
                IdDocumento : 0,
                Contenido: imgFotografia.attr('src'),
                Nombre: filefotografia[0].files[0].name
            });
        }

        if (fileidentificacionfrente[0].files.length > 0) {
            arrDocumentos.push({
                IdTipoDocumento: 1,
                IdDocumento: 0,
                Contenido: imgFrente.attr('src'),
                Nombre: fileidentificacionfrente[0].files[0].name
            });
        }

        if (fileidentificacionreverso[0].files.length > 0) {
            arrDocumentos.push({
                IdTipoDocumento: 1,
                IdDocumento: 0,
                Contenido: imgReverso.attr('src'),
                Nombre: fileidentificacionreverso[0].files[0].name
            });
        }

        if (filecomprobantedomicilio[0].files.length > 0) {
            arrDocumentos.push({
                IdTipoDocumento: 1,
                IdDocumento: 0,
                Contenido: imgDomicilio.attr('src'),
                Nombre: filecomprobantedomicilio[0].files[0].name
            });
        }

        return arrDocumentos;
    },
    nuevo: () => {

    },
    edit: (idPrestamo) => {

    },
    accionesBotones: () => {
        $('#cboTipoCliente').change(function () {
            var params = {};
            params.path = window.location.hostname;
            params.id = $(this).val();
            params = JSON.stringify(params);

            $("#txtCantidadPrestamo").val('');
            $("#txMaximoPorRenovacion").val('');

            $.ajax({
                type: "POST",
                url: "/pages/Config/CustomerTypes.aspx/GetItem",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var lo_TipoCliente = msg.d;
                    $("#txtCantidadPrestamo").val(lo_TipoCliente.PrestamoInicialMaximo);
                    $("#txMaximoPorRenovacion").val(lo_TipoCliente.CantidadParaRenovar);
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        });

        $("#UcDocumentacionCliente_filefotografia").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionCliente_imgFotografia').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionCliente_fileidentificacionfrente").change(function () {
            loans.loadPreviewImg(this,
                 function (e) {
                     $('#UcDocumentacionCliente_imgFrente').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionCliente_fileidentificacionreverso").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionCliente_imgReverso').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionCliente_filecomprobantedomicilio").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionCliente_imgDomicilio').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionAval_filefotografia").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionAval_imgFotografia').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionAval_fileidentificacionfrente").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionAval_imgFrente').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionAval_fileidentificacionreverso").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionAval_imgReverso').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionAval_filecomprobantedomicilio").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionAval_imgDomicilio').attr('src', e.target.result);
                });
        })

        $("#btnGuardarCliente").click(function (e) {
            var params = {
                Request: {}
            };

            params.Request.Prestamo = loans.getPrestamo();
            params.Request.DocumentosAval = loans.getDocumentos('UcDocumentacionAval');
            params.Request.DocumentosCliente = loans.getDocumentos('UcDocumentacionCliente');
            params.Request.Cliente = loans.getDatosPersona('UcCliente');
            params.Request.Aval = loans.getDatosPersona('UcAval');
            params.path = window.location.hostname;
            params.idUsuario = document.getElementById('txtIdUsuario').value;

            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "/pages/Loans/LoanApprove.aspx/SaveCustomerOrAval",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var lo_TipoCliente = msg.d;
                  
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        })
    }
}

window.addEventListener('load', () => {
    loans.init();
    loans.accionesBotones();
});
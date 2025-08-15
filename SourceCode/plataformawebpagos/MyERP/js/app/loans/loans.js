'use strict';
let date = new Date();
let descargas = "Prestamo_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '13';


const loans = {
    init: () => {
        loans.idPrestamo = parseInt($("#hfIdPrestamo").val());
        loans.idPrestamo = isNaN(loans.idPrestamo) ? 0 : loans.idPrestamo;
        loans.idAval = -1;
        loans.idCliente = -1;
        loans.arrDocumentosCliente = {};
        loans.arrDocumentosAval = {};
        loans.arrGarantias = {};

        $("#nav-aprobacion-supervisor-tab").hide();
        $("#nav-aprobacion-ejecutivo-tab").hide();
        $("#btnRechazar").hide();
        $("#btnAprobar").hide();
        $("#dvBotonAgregarGarantia").hide();

        const today = moment().format('YYYY-MM-DD');
        $("#txtFechaSolicitud").val(today);
        loans.loadTipoClientes(function (result) {
            loans.cargaComboClientes(result); 
            if (loans.idPrestamo > 0) {
                loans.detail(loans.idPrestamo);
            }
        });
    },
    getFechaSolicitud: () => {
        return $("#txtFechaSolicitud").val();
    },
    obtenerContadores: (IdCliente) => {
        var params = {};
        params.path = "connbd";
        params.IdCliente = IdCliente;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanApprove.aspx/ObtenrContadoresCliente",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var oResponse = msg.d;

                $("#txtVecesComoAval").val(oResponse.iContadorVecesAval);
                $("#txtPrestamosCompletados").val(oResponse.iContadorCompletados);
                $("#txtPrestamosRechazados").val(oResponse.iContadorRechazados);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        }); 
    },
    cargaComboClientes(msg) {
        var llst_TipoClientes = msg.d;
        let opcion = '<option value="">Seleccione...</option>';

        for (let i = 0; i < llst_TipoClientes.length; i++) {
            let lo_TipoCliente = llst_TipoClientes[i];
            opcion += `<option value = '${lo_TipoCliente.IdTipoCliente}' > ${lo_TipoCliente.NombreTipoCliente}</option > `;
        }

        $('#cboTipoCliente').html(opcion);
    },
    loadTipoClientes: (funcion) => {
        var params = {};
        params.path = "connbd";
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
            success: funcion,
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    loadPreviewImg: (upload,  funcion) => {
        if (upload.files && upload.files[0]) {
            var reader = new FileReader();
            reader.onload = funcion;
            reader.readAsDataURL(upload.files[0]);
        } else {
            utils.toast('Debe cargar la imagen', 'info')
        }
    },
    obtenerGarantias: (funcion) => {
        var params = {};
        params.path = "connbd";
        params.idPrestamo = loans.idPrestamo;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanApprove.aspx/ListadoGarantias",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: funcion,
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    loadTableGarantias: (data) => {
        let table = $('#tableGarantias').DataTable({
            footerCallback: function (row, data, start, end, display) {
                var lf_TotalMontos = 0;;

                $.each(data, function (index, oMontoGarantia) {
                    lf_TotalMontos += oMontoGarantia.costo;
                })

                $("#thMontoTotal").html("$" + lf_TotalMontos.toLocaleString('es-MX'));
            },
            destroy: true,
            processing: true,
            order: [],
            searching: false,
            pagingType: 'full_numbers',
            bLengthChange: false,
            data: data,
            columns: [
                { data: 'nombre' },
                { data: 'numero_serie', className: 'text-center' },
                {
                    data: 'costo', className: 'text-center', render: function (datum, type, row) {
                        return '$ ' + row.costo.toLocaleString('es-MX')
                    }
               },
                {
                    data: 'fotografia', className: 'text-center', render: function (datum, type, row) {
                        return row.fotografia ? "<img class='rounded' src='/Uploads/Prestamos/" + row.id_prestamo +"/" + row.fotografia + "' style='width:170px'/>" : '';
                    }
                },
                {
                    data: '',className: 'text-center', render: function (datum, type, row) {
                        return "<a class='rounded btn btn-danger text-white eliminarGarantia' data-id='" + row.id_garantia_prestamo + "''><i class='fa fa-trash'></i></a>";
                    }
                }
            ],
            "language": textosEsp,
            "columnDefs": [
                {
                    "targets": [-1],
                    "orderable": false
                },
            ],
            dom: 'frBtipl',
            buttons: [
                {
                    extend: 'excelHtml5',
                    title: descargas,
                    text: 'Xls', className: 'excelbtn'
                },
                {
                    extend: 'pdfHtml5',
                    title: descargas,
                    text: 'Pdf', className: 'pdfbtn'
                }
            ]
        });
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
    setPrestamo: (oPrestamo) => {
        loans.idPrestamo = oPrestamo.IdPrestamo;
        $("#lblFechaSolicitud").html(moment(oPrestamo.FechaSolicitud).format('YYYY-MM-DD'));
        $("#txtCantidadPrestamo").val(oPrestamo.Monto);
        $("#txMaximoPorRenovacion").val(oPrestamo.MontoPorRenovacion); 
        $("#cboTipoCliente").val(oPrestamo.IdTipoCliente);

        $("#txtNotaAprobacion").val(oPrestamo.NotasGenerales);
        $("#txtUbicacionReconfirmar").val(oPrestamo.IdTipoCliente);
        $("#txtNotaAprobacionEjecutivo").val(oPrestamo.NotasEjecutivo);
    },
    getPrestamo: () => {
        var oPrestamo = {};
        oPrestamo.IdPrestamo = loans.idPrestamo;
        oPrestamo.FechaSolicitud = moment($("#lblFechaSolicitud").html()).format('YYYY-MM-DD');
        oPrestamo.IdTipoCliente = $("#cboTipoCliente").val();
        oPrestamo.Monto = $("#txtCantidadPrestamo").val();
        oPrestamo.MontoPorRenovacion = $("#txMaximoPorRenovacion").val(); 

        return oPrestamo;
    },
    setDatosPersona: (oDatosPersona, control) => {
        $("#" + control + "_txtCURP").val(oDatosPersona.Curp);
        $("#" + control + "_txtNombre").val(oDatosPersona.Nombre);
        $("#" + control + "_txtPrimerApellido").val(oDatosPersona.PrimerApellido);
        $("#" + control + "_txtSegundoApellido").val(oDatosPersona.SegundoApellido);
        $("#" + control + "_txtTelefono").val(oDatosPersona.Telefono);
        $("#" + control + "_txtOcupacion").val(oDatosPersona.Ocupacion);
        loans.setDireccion(oDatosPersona.direccion, control);
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

        var imgFotografia = $("#" + control + "_imgDocumento1");
        var imgFrente = $("#" + control + "_imgDocumento2");
        var imgReverso = $("#" + control + "_imgDocumento3");
        var imgDomicilio = $("#" + control + "_imgDocumento4");

        var hfIdDocumento1 = $("#" + control + "_hfIdDocumento1");
        var hfIdDocumento2 = $("#" + control + "_hfIdDocumento2");
        var hfIdDocumento3 = $("#" + control + "_hfIdDocumento3");
        var hfIdDocumento4 = $("#" + control + "_hfIdDocumento4");

        if (filefotografia[0].files.length > 0) {
            arrDocumentos.push({
                IdTipoDocumento: 1,
                IdDocumento: $.isNumeric(hfIdDocumento1.val()) ? parseInt(hfIdDocumento1.val()) : 0,
                Contenido: imgFotografia.attr('src'),
                Nombre: filefotografia[0].files[0].name
            });
        }

        if (fileidentificacionfrente[0].files.length > 0) {
            arrDocumentos.push({
                IdTipoDocumento: 2,
                IdDocumento: $.isNumeric(hfIdDocumento2.val()) ? parseInt(hfIdDocumento2.val()) : 0,
                Contenido: imgFrente.attr('src'),
                Nombre: fileidentificacionfrente[0].files[0].name
            });
        }

        if (fileidentificacionreverso[0].files.length > 0) {
            arrDocumentos.push({
                IdTipoDocumento: 3,
                IdDocumento: $.isNumeric(hfIdDocumento3.val()) ? parseInt(hfIdDocumento3.val()) : 0,
                Contenido: imgReverso.attr('src'),
                Nombre: fileidentificacionreverso[0].files[0].name
            });
        }

        if (filecomprobantedomicilio[0].files.length > 0) {
            arrDocumentos.push({
                IdTipoDocumento: 4,
                IdDocumento: $.isNumeric(hfIdDocumento4.val()) ? parseInt(hfIdDocumento4.val()) : 0,
                Contenido: imgDomicilio.attr('src'),
                Nombre: filecomprobantedomicilio[0].files[0].name
            });
        }

        return arrDocumentos;
    },
    detail: (idPrestamo) => {
        var params = {};
        params.path = "connbd";
        params.Id = idPrestamo;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanApprove.aspx/DetallePrestamo",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var lo_Prestamo = msg.d;
                loans.idAval = lo_Prestamo.Prestamo.IdAval;
                loans.idCliente = lo_Prestamo.Prestamo.IdCliente;          

                loans.obtenerContadores(loans.idCliente);
                loans.setDatosPersona(lo_Prestamo.Cliente, 'UcCliente');
                loans.setDatosPersona(lo_Prestamo.Aval, 'UcAval');
                loans.setPrestamo(lo_Prestamo.Prestamo);
                loans.obtenerGarantias(function (msg) {
                    loans.arrGarantias = msg.d;
                    loans.loadTableGarantias(loans.arrGarantias);

                    $.each(lo_Prestamo.DocumentosCliente, function (index, oDocumento) {
                        $('#UcDocumentacionCliente_hfIdDocumento' + oDocumento.IdTipoDocumento).attr('src', oDocumento.IdDocumento);
                        $('#UcDocumentacionCliente_imgDocumento' + oDocumento.IdTipoDocumento).attr('src', oDocumento.Contenido);
                    });
                    $.each(lo_Prestamo.DocumentosAval, function (index, oDocumento) {
                        $('#UcDocumentacionCliente_hfIdDocumento' + oDocumento.IdTipoDocumento).attr('src', oDocumento.IdDocumento);
                        $('#UcDocumentacionAval_imgDocumento' + oDocumento.IdTipoDocumento).attr('src', oDocumento.Contenido);
                    });

                    if (lo_Prestamo.Prestamo.IdStatusPrestamo === 1) {
                        $("#frmAval input").prop("disabled", true);
                        $("#frmCustomer input").prop("disabled", true);
                        $("#nav-aprobacion-supervisor-tab").show();
                        $("#dvBotonAgregarGarantia").show();
                        $("#btnRechazar").show();
                        $("#btnAprobar").show();
                    }
                    else if (lo_Prestamo.Prestamo.IdStatusPrestamo === 2) {
                        $("#frmAval input").prop("disabled", true);
                        $("#frmCustomer input").prop("disabled", true);
                        $("#nav-aprobacion-ejecutivo-tab").show();
                        $("#btnRechazar").show();
                        $("#btnAprobar").show();
                        $("#tableGarantias tbody tr a").prop("style", "display:none");
                    }
                    else {
                        $("#frmAval input").prop("disabled", true);
                        $("#frmCustomer input").prop("disabled", true);
                        $("#frmAprobacion input").prop("disabled", true);
                        $("#frmAprobacionEjecutivo input").prop("disabled", true);
                        $("#frmAprobacion textarea").prop("disabled", true);
                        $("#frmAprobacionEjecutivo textarea").prop("disabled", true);
                        $("#btnGuardarCliente").hide();
                        $("#nav-aprobacion-supervisor-tab").show();
                        $("#nav-aprobacion-ejecutivo-tab").show();
                        $("#tableGarantias tbody tr a").prop("style", "display:none");
                    }
                });
                
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    getLocation: (control) => {
        var options = {
            enableHighAccuracy: true,
            timeout: 5000,
            maximumAge: 0
        };

        function success(pos) {
            var crd = pos.coords;
            $("#" + control).val(`${crd.latitude}, ${crd.longitude}`);
        };

        function error(err) {
            console.warn('ERROR(' + err.code + '): ' + err.message);
        };

        navigator.geolocation.getCurrentPosition(success, error, options);
    },
    guardaRechazo: () => {
        var params = {};
        params.oPrestamo = {
            IdPrestamo: loans.idPrestamo,
            NotasGenerales: $("#txtNotaAprobacion").val(),
            UbicacionConfirmada: $("#txtUbicacionReconfirmar").val(),
        };
        params.path = "connbd";

        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanApprove.aspx/RechazoPrestamo",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var oResponse = msg.d;
                if (oResponse.CodigoError <= 0) {
                    utils.toast('El prestamo fue rechazado correctamente', 'ok');

                    setTimeout(function () {
                        window.location = '/pages/Loans/LoanRequest.aspx';
                    }, 500)
                }
                else {
                    utils.toast(oResponse.MensajeError, 'error');
                }
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    guardarAprobacionSupervisor: () => {
        var params = {};
        params.oPrestamo = {
            IdPrestamo: loans.idPrestamo,
            IdTipoCliente: $("#cboTipoCliente").val(),
            NotasGenerales: $("#txtNotaAprobacion").val(),
            UbicacionConfirmada: $("#txtUbicacionReconfirmar").val(),
        };
        params.fMontoGarantia = 0;
        params.path = "connbd";
        $.each(loans.arrGarantias, function (index, garantia) {
            params.fMontoGarantia += garantia.costo;
        });
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanApprove.aspx/AprobacionSupervisor",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var oResponse = msg.d;
                if (oResponse.CodigoError <= 0) {
                    utils.toast('El prestamo fue aprobado por el supervisor correctamente', 'ok');

                    setTimeout(function () {
                        window.location = '/pages/Loans/LoanRequest.aspx';
                    }, 500)
                }
                else {
                    utils.toast(oResponse.MensajeError, 'error');
                }
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    guardaAprobacionEjecutivo: () => {
        var frmAprobacionEjecutivo = $("#frmAprobacionEjecutivo");
        if (frmAprobacionEjecutivo.valid()) {
            var params = {};
            params.path = "connbd";
            params.IdPrestamo = loans.idPrestamo;
            params.sNotaEjecutivo = $("#txtNotaAprobacionEjecutivo").val();
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "/pages/Loans/LoanApprove.aspx/AprobacionEjecutivo",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var oResponse = msg.d;
                    if (oResponse.CodigoError <= 0) {
                        utils.toast('El prestamo fue aprobado por el ejecutivo', 'ok');
                        setTimeout(function () {
                            window.location = '/pages/Loans/LoanRequest.aspx';
                        }, 500)
                    }
                    else {
                        utils.toast(oResponse.MensajeError, 'error');
                    }
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        }
    },
    guardarClienteAval: () => {
        var params = {
            Request: {}
        };
        params.Request.Prestamo = loans.getPrestamo();
        params.Request.DocumentosAval = loans.getDocumentos('UcDocumentacionAval');
        params.Request.DocumentosCliente = loans.getDocumentos('UcDocumentacionCliente');
        params.Request.Cliente = loans.getDatosPersona('UcCliente');
        params.Request.Aval = loans.getDatosPersona('UcAval');
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.Request.Cliente.idCliente = loans.idCliente;
        params.Request.Aval.idCliente = loans.idAval;

        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanApprove.aspx/SaveCustomerOrAval",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var oResponse = msg.d;
                if (oResponse.CodigoError <= 0) {
                    utils.toast('Los datos del prestamo fue actualizado correctamente', 'ok');
                    setTimeout(function () {
                        window.location = '/pages/Loans/LoanRequest.aspx';
                    }, 500)
                }
                else {
                    utils.toast(oResponse.MensajeError, 'error');
                }
                
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    accionesBotones: () => {
        $(document).on("click", ".btnReloadLocation", function () {
            var sControl = $(this).attr('id').substring(0, $(this).attr('id').indexOf('_'));
            loans.getLocation(sControl+ '_txtUbicacion');
        });

        $("#btnConfirmarUbicacion").click(function () {
            loans.getLocation('txtUbicacionReconfirmar');
        });

        $(document).on('click','.eliminarGarantia', function () {
            var params = {};
            params.path = "connbd";
            params.Id = $(this).attr('data-id');
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "/pages/Loans/LoanApprove.aspx/DeleteGarantia",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var lo_Salida = msg.d;
                    if (lo_Salida.CodigoError <= 0) {
                        loans.obtenerGarantias(function (msg) {
                            var listaGarantias = msg.d;
                            $("#frmGarantias")[0].reset();
                            $("#imgImagenGarantia").attr('src', '');

                            loans.arr = listaGarantias;
                            loans.loadTableGarantias(listaGarantias);
                        });
                    }
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        })

        $("#btnAgregarGarantia").click(function () {
            var frmGarantias = $("#frmGarantias");

            if (frmGarantias.valid()) {
                var params = {};
                params.path = "connbd";
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params.oGarantia = {
                    id_prestamo: loans.idPrestamo,
                    nombre: $("#txtNombreGarantia").val(),
                    numero_serie: $("#txtNumeroSerie").val(),
                    costo: $("#txtCosto").val(),
                    fotografia: $("#imgImagenGarantia").attr('src')
                }
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "/pages/Loans/LoanApprove.aspx/SaveGarantia",
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        var lo_Salida = msg.d;
                        if (lo_Salida.CodigoError <= 0) {
                            loans.obtenerGarantias(function (msg) {
                                var listaGarantias = msg.d;
                                $("#frmGarantias")[0].reset();
                                $("#imgImagenGarantia").attr('src', '');
                                loans.arrGarantias = listaGarantias;
                                loans.loadTableGarantias(listaGarantias);
                            });
                        }
                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }
                });
            }
        })

        $('#cboTipoCliente').change(function () {
            var params = {};
            params.path = "connbd";
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

        $("#flImagenGarantia").change(function(){
            loans.loadPreviewImg(this,
                function (e) {
                    $('#imgImagenGarantia').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionCliente_filefotografia").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionCliente_imgDocumento1').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionCliente_filefotografia").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionCliente_imgDocumento1').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionCliente_fileidentificacionfrente").change(function () {
            loans.loadPreviewImg(this,
                 function (e) {
                     $('#UcDocumentacionCliente_imgDocumento2').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionCliente_fileidentificacionreverso").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionCliente_imgDocumento3').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionCliente_filecomprobantedomicilio").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionCliente_imgDocumento4').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionAval_filefotografia").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionAval_imgDocumento1').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionAval_fileidentificacionfrente").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionAval_imgDocumento2').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionAval_fileidentificacionreverso").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionAval_imgDocumento3').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionAval_filecomprobantedomicilio").change(function () {
            loans.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionAval_imgDocumento4').attr('src', e.target.result);
                });
        })

        $("#btnAprobar").click(function (e) {
            e.preventDefault();
            var li_tab_index = $('#nav-tab-prestamos .active').attr('data-id');

            switch (li_tab_index) {
                //Aprobacion de supervisor
                case "3":
                    loans.guardarAprobacionSupervisor();
                    break;
                //Aprobacion de  ejecutivo
                case "4":
                    loans.guardaAprobacionEjecutivo();
                    break;
            }
        })

        $("#btnRechazar").click(function (e) {
            e.preventDefault();
            loans.guardaRechazo();
        })

        $("#btnGuardarCliente").click(function (e) {
            loans.guardarClienteAval();
        })

        $(document).on("keyup", ".curp-persona", function () {
            var txtCurp = $(this);
            txtCurp.val(txtCurp.val().toUpperCase());
        });

        $(document).on("change",".curp-persona" ,function () {
            var sControl = $(this).attr('id').substring(0, $(this).attr('id').indexOf('_'));
            var params = {};
            params.path = "connbd";
            params.sCURP = $(this).val();
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "/pages/Loans/LoanApprove.aspx/BuscarClientePorCURP",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var oCliente = msg.d;
                    if (oCliente.IdCliente > 0) {

                        if (sControl === 'UcCliente') { loans.idCliente = oCliente.IdCliente; }
                        if (sControl === 'UcAval') { loans.idAval = oCliente.IdCliente; }
                        if (loans.idCliente > 0) {
                            loans.obtenerContadores(loans.idCliente);
                        }

                        utils.toast('El CURP fue encontrado', 'ok');
                        loans.setDatosPersona(oCliente, sControl)
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
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
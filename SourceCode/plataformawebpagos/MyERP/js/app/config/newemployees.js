'use strict';
let date = new Date();
let descargas = "Empleados_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '8';


const POSICION_DIRECTOR = 1;
const POSICION_COORDINADOR = 2;
const POSICION_EJECUTIVO = 3;
const POSICION_SUPERVISOR = 4;
const POSICION_PROMOTOR = 5;

const employee = {
    init: () => {
        employee.idSeleccionado = $.isNumeric($("#hfEmpleadoId").val())
            ? parseInt($("#hfEmpleadoId").val())
            : -1;
        employee.loadComboPosicion();
        employee.loadComboPlaza();
        employee.loadComboEmployeesByPosicion(POSICION_EJECUTIVO, '#cboEjecutivo');
        employee.loadComboEmployeesByPosicion(POSICION_SUPERVISOR, '#cboSupervisor');

        if (employee.idSeleccionado > 0) {
            employee.setDatosPersona('UcAval');
        }
        else {
            $("#dtpFechaIngreso").val(moment(Date.now()).format('YYYY-MM-DD'));
        }
    },
    getDocumentos: (control) => {
        var arrDocumentos = [];
        var filefotografia = $("#" + control + "_filefotografia");
        var fileidentificacionfrente = $("#" + control + "_fileidentificacionfrente");
        var fileidentificacionreverso = $("#" + control + "_fileidentificacionreverso");
        var filecomprobantedomicilio = $("#" + control + "_filecomprobantedomicilio");

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
    setDatosPersona: (oDatosPersona, control) => {
        $("#" + control + "_txtCURP").val(oDatosPersona.Curp);
        $("#" + control + "_txtNombre").val(oDatosPersona.Nombre);
        $("#" + control + "_txtPrimerApellido").val(oDatosPersona.PrimerApellido);
        $("#" + control + "_txtSegundoApellido").val(oDatosPersona.SegundoApellido);
        $("#" + control + "_txtTelefono").val(oDatosPersona.Telefono);
        $("#" + control + "_txtOcupacion").val(oDatosPersona.Ocupacion);
        employee.setDireccion(oDatosPersona.direccion, control);
    },
    getUser: () => {
        var oUser = {};
        oUser.IdEmpleado = employee.idSeleccionado;
        oUser.Login = $('#txtNombreUsuario').val();
        oUser.Password = $('#txtPassword').val();
        oUser.IdTipoUsuario = $('#comboPosicion').val();

        return oUser;
    },
    getColaborador: () => {
        var oColaborador = {};
        oColaborador.IdEmpleado = employee.idSeleccionado;
        oColaborador.FechaIngreso = moment($('#dtpFechaIngreso').val()).format('YYYY-MM-DD');
        oColaborador.fecha_baja = moment($('#dtpFechaBaja').val()).isValid()
            ? moment($('#dtpFechaBaja').val()).format('YYYY-MM-DD')
            : null;
        oColaborador.IdPosicion = $('#cboPuesto').val();
        oColaborador.IdSupervisor = $('#cboSupervisor').val();
        oColaborador.IdPlaza = $('#cboPlaza').val();
        oColaborador.IdEjecutivo = $('#cboEjecutivo').val();
        oColaborador.CURP = $('#UcColaborador_txtCURP').val();
        oColaborador.Telefono = $('#UcColaborador_txtTelefono').val();
        oColaborador.Ocupacion = $('#UcColaborador_txtOcupacion').val();
        oColaborador.PrimerApellido = $('#UcColaborador_txtPrimerApellido').val();
        oColaborador.SegundoApellido = $('#UcColaborador_txtSegundoApellido').val();
        oColaborador.Nombre = $('#UcColaborador_txtNombre').val();
        oColaborador.IdPosicion = $.isNumeric($('#cboPuesto').val()) ? parseInt($('#cboPuesto').val())
            : 0;
        oColaborador.IdSupervisor = $.isNumeric($('#cboSupervisor').val()) ? parseInt($('#cboSupervisor').val())
            : 0;
        oColaborador.IdEjecutivo = $.isNumeric($('#cboEjecutivo').val()) ? parseInt($('#cboEjecutivo').val())
            : 0;
        oColaborador.IdPlaza = $.isNumeric($('#cboPlaza').val()) ? parseInt($('#cboPlaza').val())
            : 0;
        oColaborador.Direccion = employee.getDireccion('UcColaborador');
        oColaborador.limite_venta_ejercicio = $('#txtLimiteVentaPorEjercicio').val();
        oColaborador.limite_incremento_ejercicio = $('#txtLimiteIncrementoPorEjercicio').val();
        oColaborador.fizcalizable = $('#cboFiscalzable').val() === '1' ? true : false;
        oColaborador.NotaFoto = $('#txtNotaFoto').val();

        return oColaborador;
    },
    getDatosPersona: (control) => {
        var oDatosPersona = {};

        oDatosPersona.Curp = $("#" + control + "_txtCURP").val();
        oDatosPersona.Nombre = $("#" + control + "_txtNombre").val();
        oDatosPersona.PrimerApellido = $("#" + control + "_txtPrimerApellido").val();
        oDatosPersona.SegundoApellido = $("#" + control + "_txtSegundoApellido").val();
        oDatosPersona.Telefono = $("#" + control + "_txtTelefono").val();
        oDatosPersona.Ocupacion = $("#" + control + "_txtOcupacion").val();
        oDatosPersona.direccion = employee.getDireccion(control);

        return oDatosPersona;
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
    loadComboEmployeesByPosicion: (idTipoEmpleado, control) => {

        var params = {};
        params.path = window.location.hostname;
        params.idTipoEmpleado = idTipoEmpleado;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/NewEmployee.aspx/GetListaItemsEmpleadoByPosicion",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdEmpleado}' > ${item.Nombre}</option > `;

                }

                $(`${control}`).html(opcion);

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
    loadComboPosicion: () => {
        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/NewEmployee.aspx/GetListaItemsPosiciones",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];
                    opcion += `<option value = '${item.IdPosicion}' > ${item.Nombre}</option > `;
                }

                $('#cboPuesto').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },
    loadPreviewImg: (upload, funcion) => {
        if (upload.files && upload.files[0]) {
            var reader = new FileReader();
            reader.onload = funcion;
            reader.readAsDataURL(upload.files[0]);
        } else {
            utils.toast('Debe cargar la imagen', 'info')
        }
    },
    loadComboPlaza: () => {
        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/NewEmployee.aspx/GetListaItemsPlazas",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];
                    opcion += `<option value = '${item.IdPlaza}' > ${item.Nombre}</option > `;
                }

                $('#cboPlaza').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    accionesBotones: () => {
        $(document).on("click", ".btnReloadLocation", function () {
            var sControl = $(this).attr('id').substring(0, $(this).attr('id').indexOf('_'));
            employee.getLocation(sControl + '_txtUbicacion');
        });

        $('#btnGuardar').on('click', (e) => {
            e.preventDefault();

            let params = {};
            params.path = window.location.hostname;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.oRequest = {
                Colaborador: employee.getColaborador(),
                User: employee.getUser(),
                Aval: employee.getDatosPersona('UcAval'),
                DocumentosAval: employee.getDocumentos('UcDocumentacionAval'),
                DocumentosColaborador: employee.getDocumentos('UcDocumentacionColaborador')
            };

            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: `/pages/Config/NewEmployee.aspx/Save`,
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;
                    if (valores.CodigoError === 1) {
                        utils.toast(valores.MensajeError, 'error');
                    }
                    else {
                        window.location = '/pages/Config/Employees.aspx';
                    }
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    utils.toast(mensajesAlertas.errorGuardar, 'error');
                }
            });
        });

        $("#UcDocumentacionColaborador_filefotografia").change(function () {
            employee.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionColaborador_imgDocumento1').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionColaborador_filefotografia").change(function () {
            employee.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionColaborador_imgDocumento1').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionColaborador_fileidentificacionfrente").change(function () {
            employee.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionColaborador_imgDocumento2').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionColaborador_fileidentificacionreverso").change(function () {
            employee.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionColaborador_imgDocumento3').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionColaborador_filecomprobantedomicilio").change(function () {
            employee.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionColaborador_imgDocumento4').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionAval_filefotografia").change(function () {
            employee.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionAval_imgDocumento1').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionAval_fileidentificacionfrente").change(function () {
            employee.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionAval_imgDocumento2').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionAval_fileidentificacionreverso").change(function () {
            employee.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionAval_imgDocumento3').attr('src', e.target.result);
                });
        })

        $("#UcDocumentacionAval_filecomprobantedomicilio").change(function () {
            employee.loadPreviewImg(this,
                function (e) {
                    $('#UcDocumentacionAval_imgDocumento4').attr('src', e.target.result);
                });
        })
    }
}

window.addEventListener('load', () => {
    employee.init();
    employee.accionesBotones();
});

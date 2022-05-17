
var accion = "";
var idSeleccionado = "-1";
var table;
var usuario = '';



let nombreUsuario = null;
let idTipoUsuario = null;
let idUsuario = null;


var mensajesAlertas = {
    errorSubirCsv: 'Debe ingregar un archivo de tipo csv.',
    errorGuardar: 'Se ha producido un error al almacenar los datos. Los datos no fueron almacenados.',
    exitoGuardar: 'Los datos se almacenaron correctamente.',
    errorInesperado: 'Se ha producido un error inesperado. Por favor intente de nuevo.',
    exitoEliminar: 'El registro se eliminó correctamente.',
    exitoCancelarRefaccion: 'La solicitud de refacción se canceló correctamente.',
    exitoEnviar: 'El registro se cambio de status correctamente.',
    exitoFinalizar: 'La orden de trabajo se finalizó correctamente.',
    exitoCambiarStatus: 'La orden de trabajo se actualizó correctamente.',
    errorEliminar: 'No se pudo eliminar el registro.',
    infraccionNoDisponible: 'No se pudo cargar la infracción seleccionada.',
    exitoPasswordModificada: 'Contraseña modificada correctamente.',
    exitoOperacion: 'Operación finalizada correctamente.',
    exitoCancelarSolicitudCombustible: 'Operación finalizada correctamente.',
    errorSeleccionar: 'Primero debe seleccionar una infracción.',
    errorDescargoPendiente: 'La infraccion seleccionada ya cuenta con un descargo pendiente.',

    errorTipoArchivo: 'Debe ingregar un archivo de tipo csv.',
    errorSeleccionarCampo: 'Debe seleccionar un campo.',
    exitoEntregar: 'Se registró la entrega de combustible correctamente.',
    exitoAprobar: 'Se aprobó la solicitud de combustible correctamente.',

    errorNoStatusPendiente: 'El descargo ya no se encuentra en status pendiente.',
    errorSeleccionarFecha: 'Debe indicar la nueva fecha.',
    confirmacionCancelarRefaccion: 'Se cancelará la solicitud de refacción seleccionada No. {numrefaccion} ¿Desea continuar? ',
    errorSeleccionarEquipo: 'Debe seleccionar un equipo',
    errorSeleccionarObra: 'Debe seleccionar una obra',
    errorSeleccionarUsuarioDiagnostico: 'Debe seleccionar un usuario para diagnóstico',
    errorSeleccionarUsuarioColaborador: 'Debe seleccionar un usuario',
    errorSeleccionarProveedor: 'Debe seleccionar un proveedor',
    errorSeleccionarProveedorAlEnviar: 'La requisición actual no cuenta con un proveedor seleccionado, debe editarla y asignarle un proveedor',
    errorSeleccionarProveedorUsuario: 'Debe seleccionar un proveedor a quien se será asignado este usuario',
    errorSeleccionarMarca: 'Debe seleccionar una marca-proveedor',
    errorSeleccionarOperador: 'Debe seleccionar un valor para el campo <strong>Operador actual</strong> ',
    exitoResetStatus: 'Se resetearon los status de las Ots correctamente.',
    errorResetStatus: 'No se pudo completar la operación.',
    exitoAsignarColaboracion: 'Se ha registrado la relación de colaboración correctamente.',
    exitoFinalizarColaboracion: 'Se ha finalizado la relación de colaboración correctamente.',
    errorValorOrometro: 'Debe indicar el valor para el campo Horómetro/odómetro.',

    exitoGenerarOtsManto: 'Se generaron correcamente las Ots de mantenimiento.',
    errorGenerarOtsManto: 'No se pudo completar la operación correctamente.',


    errorSeleccionarUbicacion: 'No se pudo completar la operación correctamente.',
    errorSolicitudesCombustible: 'Debe asignar valores correctos (cantidades con valores positivos) a cada fila de la tabla de solicitudes.',
    errorSolicitudesCombustibleCinchoActual: 'Debe asignar valor correcto para cincho actual (cantidades con valores positivos) a la fila a entregar.',
    errorSolicitudesLitrosSurtidos: 'Debe asignar valor correcto para litros surtidos(cantidades con valores positivos) a la fila a entregar.',
    errorSolicitudesHorometroEntrega: 'Debe asignar valor correcto para odómetro/horómetro en entrega (cantidades con valores positivos) a la fila a entregar.',
    errorSolicitudesCombustibleOrometroAprobacion: 'Debe asignar valor correcto para odómetro/horómetro en aprobación (cantidades con valores positivos) a la fila a aprobar.',
    errorSolicitudesCombustibleLitrosAprobacion: 'Debe asignar valor correcto para litros en entrega (cantidades con valores positivos) a la fila a aprobar.'


};

var textosEsp =
{
    "sProcessing": "Procesando...",
    "sLengthMenu": "Mostrar _MENU_  registros",
    "sZeroRecords": "No se encontraron resultados",
    "sEmptyTable": "Ningún dato disponible en esta tabla",
    "sInfo": "Registros _START_ al _END_ de _TOTAL_ registros",
    "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
    "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
    "sInfoPostFix": "",
    "sSearch": "Buscar:",
    "sUrl": "",
    "sInfoThousands": ",",
    "sLoadingRecords": "Cargando...",
    "oPaginate": {
        "sFirst": "Primero",
        "sLast": "Último",
        "sNext": "Siguiente",
        "sPrevious": "Anterior"
    },
    "oAria": {
        "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
        "sSortDescending": ": Activar para ordenar la columna de manera descendente"
    }
};


$(document).ready(function () {

    nombreUsuario = document.getElementById('txtUsuario').value;
    idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
    idUsuario = document.getElementById('txtIdUsuario').value;

    $('#nombreUsuario').text(nombreUsuario);


    let params = {};
    params.path = window.location.hostname;
    params.pagina = pagina;
    params.idUsuario = idUsuario;
    params.idTipoUsuario = idTipoUsuario;
    params.nombreUsuario = nombreUsuario;
    params = JSON.stringify(params);

    $.ajax({
        type: "POST",
        url: "../../pages/Index.aspx/GetNav",
        data: params,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {
            //console.log(msg.d);

            const elementosInterfaz = msg.d;

            if (elementosInterfaz == null && pagina !== '0') {
                window.location = "../../pages/Index.aspx";
            } else {


                $('#spnMenuSuperior').empty().html(elementosInterfaz.BarraSuperior).promise().done(function () {


                    controlLateral();

                    cargarValoresConfiguracionEmpresa('4', '.nombre-empresa');//logo empresa


                    cargarValoresConfiguracionNombreSistema('8');// <title>

                    cargarValoresConfiguracionEmpresa('3', '.empresa-copy');//link y año


                });


                $('#side-main-menu').html(elementosInterfaz.BarraLateral);

            }



        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });



    function cargarValoresConfiguracionNombreSistema(id) {

        let parametros = {};
        parametros.path = window.location.hostname;
        parametros.id = id;
        parametros = JSON.stringify(parametros);

        $.ajax({
            type: "POST",
            url: "../../pages/Index.aspx/GetItemConfiguracion",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let valores = msg.d;
                
                if (valores.ValorCadena != null) {
                    document.title = valores.ValorCadena;
                } else {
                    document.title = '';
                }

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                document.title = '';

                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }

    function cargarValoresConfiguracionEmpresa(id, controlHtml) {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = id;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../../pages/Index.aspx/GetItemConfiguracion",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let valores = msg.d;

                $(controlHtml).html(valores.ValorCadena);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }

    function controlLateral() {

        //if ($(window).outerWidth() > 992) {
        //    $("nav.side-navbar").mCustomScrollbar({
        //        scrollInertia: 200
        //    });
        //}

        // Main Template Color
        var brandPrimary = '#33b35a';

        // ------------------------------------------------------- //
        // Side Navbar Functionality
        // ------------------------------------------------------ //
        $('#toggle-btn').on('click', function (e) {

            e.preventDefault();

            if ($(window).outerWidth() > 1194) {
                $('nav.side-navbar').toggleClass('shrink');
                $('.page').toggleClass('active');
            } else {
                $('nav.side-navbar').toggleClass('show-sm');
                $('.page').toggleClass('active-sm');
            }
        });
    }




});


const obtenerFechaHoraServidor = (idControl) => {
    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Proyectos.aspx/GetFechaHora",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {
            var fecha = msg.d;


            $('#' + idControl).val(fecha.FechaFormateada);
            $('#' + idControl).prop('fecha', fecha.Fecha);



        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });

}


function number_format(amount, decimals, simbolo = '$') {

    var sign = (amount.toString().substring(0, 1) === "-");

    amount += '';
    amount = parseFloat(amount.replace(/[^0-9\.]/g, ''));

    decimals = decimals || 0;

    if (isNaN(amount) || amount === 0)
        return parseFloat(0).toFixed(decimals);

    amount = '' + amount.toFixed(decimals);

    var amount_parts = amount.split('.'),
        regexp = /(\d+)(\d{3})/;

    while (regexp.test(amount_parts[0]))
        amount_parts[0] = amount_parts[0].replace(regexp, '$1' + ',' + '$2');

    var r = sign ? '-' + simbolo + ' ' + amount_parts.join('.') : simbolo + ' ' + amount_parts.join('.');

    return r;
}



var utils = {
    idDoc: -1,

    getDocumentos: (params) => {




        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoPanelRegistroFallas.aspx/GetDocumentos",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let documentos = msg.d;

                //console.log(msg.d);

                let htmlDoc = '';
                htmlDoc += '<div class = "row">';

                documentos.forEach(function (documento) {

                    let url = `'../pages/Download.ashx?path=${window.location.hostname}&id_documento=${documento.IdDocumento}'`;

                    htmlDoc += `
                            <div class="card bg-light" style="width: 18rem;">
                              <div class="card-body text-center">
                            `;

                    htmlDoc += '<a href = "#" onClick = "utils.abrirImagen(' + url + ',' + (documento.Descripcion.trim().toLowerCase().endsWith('pdf') || documento.Descripcion.trim().toLowerCase().endsWith('docx')) + ' ) " >';

                    if (documento.Descripcion.trim().toLowerCase().endsWith('gif') ||
                        documento.Descripcion.trim().toLowerCase().endsWith('png') ||
                        documento.Descripcion.trim().toLowerCase().endsWith('jpg') ||
                        documento.Descripcion.trim().toLowerCase().endsWith('jpeg') ||
                        documento.Descripcion.trim().toLowerCase().endsWith('bmp')
                    ) {
                        htmlDoc += '<img src="' + documento.Url + '" class="img-thumbnail thumb" />';
                    } else {
                        htmlDoc += '<img src="/pages/Uploads/pdf.png" class="img-thumbnail thumb" />';
                    }

                    htmlDoc += '</a>';
                    htmlDoc += `

                                <p>
                                    <small class="text-muted">
                                    (${documento.IdDocumento}) ${documento.Descripcion}
                                    </small>
                                </p>
                        `;



                    htmlDoc += (Number(idTipoUsuario) === 1)
                        ? `<div><button class="btn btn-sm btn-default eliminar-documento deshabilitable" data-iddocumento="${documento.IdDocumento}"><em class="fa fa-times"></em></button></div>`
                        : ``;

                    htmlDoc += '</div>';
                    htmlDoc += '</div>';



                });

                htmlDoc += '</div>';
                htmlDoc += '</div>';

                $('#divDocumentosExistentes').empty().append(htmlDoc);


                $('.eliminar-documento').on('click', (e) => {
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    let idDoc = e.currentTarget.dataset["iddocumento"];

                    utils.idDoc = idDoc;

                    //console.log(`eliminar-documento ${idDoc}`);

                    $('#msgEliminarImagen').html('<p>Se va a eliminar esta imagen/documento  (No.' + idDoc + '). ¿Desea continuar?</p>');
                    $('#panelEliminarImagen').modal('show');

                });




                $('#panelTabla').hide();
                $('#panelForm').show();


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });



    },


    abrirImagen: (url, pdf) => {

        if (!pdf) {
            $('#imgDoc').attr('src', url);
            $('#imgDoc').attr('src', url);
            $('#panelImagen').modal('show');
        }
        else {
            window.open(url);
        }

        //location.href = url;
    },

    postData: async (url = '', data = '') => {

        const response = await fetch(url, {
            method: 'POST', // *GET, POST, PUT, DELETE, etc.
            mode: 'cors', // no-cors, *cors, same-origin
            cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
            credentials: 'same-origin', // include, *same-origin, omit
            headers: {
                'Content-Type': 'application/json'
                // 'Content-Type': 'application/x-www-form-urlencoded',
            },
            redirect: 'follow', // manual, *follow, error
            referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
            body: JSON.stringify(data) // body data type must match "Content-Type" header
        });
        return response.json(); // parses JSON response into native JavaScript objects
    },

    getFileExtension: (filename) => {
        return filename.slice((filename.lastIndexOf(".") - 1 >>> 0) + 2);
    },


    sendFile: (file, nombreArchivo, idItem, tipo) => {


        var files = file;
        var fileName = files.name;
        var extension = utils.getFileExtension(fileName);
        var idUsuario = idUsuario;




        var formData = new FormData();
        formData.append('file', file);
        formData.append('id', idItem);//
        formData.append('pagina', window.location.pathname);//
        formData.append('path', window.location.hostname);//
        formData.append('extension', extension);//
        formData.append('descripcion', fileName);//
        formData.append('tipo', tipo);//
        formData.append('idUsuario', idUsuario);//
        formData.append('nombreArchivo', nombreArchivo)


        $.ajax({
            type: 'post',
            url: '../pages/fileUploader.ashx',
            data: formData,
            success: function (status) {
                if (status != 'error') {


                }
            },
            processData: false,
            contentType: false,
            error: function () {

            }
        });
    },


    toast: (mensaje, tipo) => {
        if (tipo === 'ok') {
            setTimeout(function () {
                toastr.options = {
                    closeButton: true,
                    progressBar: true,
                    showMethod: 'slideDown',
                    timeOut: 4000
                };
                toastr.success(mensaje);

            }, 500);
        } else {
            setTimeout(function () {
                toastr.options = {
                    closeButton: true,
                    progressBar: true,
                    showMethod: 'slideDown',
                    timeOut: 4000
                };
                toastr.error(mensaje);

            }, 500);
        }
    },

    isMobile: () => {
        let check = false;
        (function (a) { if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true; })(navigator.userAgent || navigator.vendor || window.opera);
        return check;
    },

    parseURLParams: (url) => {
        //console.log(`url = ${url}`);

        var queryStart = url.indexOf("?") + 1,
            queryEnd = url.indexOf("#") + 1 || url.length + 1,
            query = url.slice(queryStart, queryEnd - 1),
            pairs = query.replace(/\+/g, " ").split("&"),
            parms = {}, i, n, v, nv;

        if (query === url || query === "") return;

        for (i = 0; i < pairs.length; i++) {
            nv = pairs[i].split("=", 2);
            n = decodeURIComponent(nv[0]);
            v = decodeURIComponent(nv[1]);

            if (!parms.hasOwnProperty(n)) parms[n] = [];
            parms[n].push(nv.length === 2 ? v : null);
        }

        return parms;
    }


}
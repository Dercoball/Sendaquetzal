'use strict';
let date = new Date();
let descargas = "Dashboard_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '79';


const otReset = {


    init: () => {

    },



    accionesBotones: () => {



        $('#btnActualizarOrometros').on('click', (e) => {
            e.preventDefault();

            console.log(`btnActualizarOrometros`);

            let parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");
            parametros = JSON.stringify(parametros);

            $.ajax({
                type: "POST",
                url: "../pages/OTReset.aspx/ActualizarOrometros",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoResetStatus, 'ok');


                        //otReset.init();


                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


        });


        $('#btnGenerarOts').on('click', (e) => {
            e.preventDefault();

            console.log(`btnGenerarOts`);

            $('#imgLoading').css('display', 'inline');
            $('#btnGenerarOts').prop('disabled', true);

            let parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");
            parametros = JSON.stringify(parametros);

            $.ajax({
                type: "POST",
                url: "../pages/OTReset.aspx/RevisaGeneracionOtsMantenimiento",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    console.log(resultado);

                    $('#imgLoading').css('display', 'none');
                    $('#btnGenerarOts').prop('disabled', false);

                    if (resultado != null) {
                        $('#msgGenerarOts').html(
                            `
                            <p style="overflow-wrap: break-word;">Se van a generar OT\'s de Mantenimiento preventivo (PM) para los siguientes equipos: ${resultado.listaEquiposStr}</p> ¿Desea continuar?
                        `);

                        $('#txtResumen').empty().append(resultado.log);

                        $('#panelGenerarOts').modal('show');

                    } else {
                        $('#spnMensajes').html(
                            `
                            No existen Ots de mantenimiento por generar.
                        `);
                        $('#panelMensajes').modal('show');

                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


        });


        $('#btnGenerarOtsAceptar').on('click', (e) => {

            e.preventDefault();

            $('#imgLoading').css('display', 'inline');
            $('#btnGenerarOts').prop('disabled', true);

            let parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");
            parametros = JSON.stringify(parametros);

            $.ajax({
                type: "POST",
                url: "../pages/OTReset.aspx/GenerarOtsMantenimiento",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    console.log(resultado);

                    $('#imgLoading').css('display', 'none');
                    $('#btnGenerarOts').prop('disabled', false);

                    if (resultado.MensajeError === '') {
                        $('#spnMensajes').html(
                            `
                            <p style="overflow-wrap: break-word;">Se generaron las siguientes Mantenimiento preventivo (PM): ${resultado.IdItem}</p>
                        `);
                        $('#panelMensajes').modal('show');

                    } else {
                        $('#spnMensajes').html(
                            `
                            No se pudieron generar las Ots. Intente nuevamente.
                        `);
                        $('#panelMensajes').modal('show');

                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });



        $('#btnResetStatus').on('click', (e) => {
            e.preventDefault();

            $('#msgResetStatus').html(
                `
                    Se van a cambiar de status a Por atender todas las ordenes de trabajo que actualmente se encuentran
                        en status En atención y que no están finalizadas ni eliminadas ¿Desea continuar?
                `);

            $('#paneResetStatus').modal('show');


        });

        $('#btnResetStatusAceptar').on('click', (e) => {

            e.preventDefault();

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/OTReset.aspx/ResetRequisiciones",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoResetStatus, 'ok');


                        otReset.init();


                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });


    }


}

window.addEventListener('load', () => {

    otReset.init();

    otReset.accionesBotones();

});



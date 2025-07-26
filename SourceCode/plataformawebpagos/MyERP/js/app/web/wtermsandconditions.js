'use strict';
let pagina = '3';



const wabout = {


    init: () => {

        $('#panelTabla').show();

        wabout.loadContent();
        

    },

    loadContent() {


        let params = {};
        params.path = "connbd";
        params.idTabla = "2";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Web/WAboutUs.aspx/LoadContent",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                let resultado = msg.d;

                tinymce.get("contenido").setContent(resultado);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });



    },


    accionesBotones: () => {



        $('#btnGuardar').click(function (e) {

            e.preventDefault();

            let params = {};
            params.path = "connbd";
            params.contenido = tinymce.get("contenido").getContent();
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.idTabla = "2";
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Web/WAboutUs.aspx/Save",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    let resultado = parseInt(msg.d);

   
                    if (resultado > 0) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                        $('#panelTabla').show();


                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'fail');


                    }

                    $('#panelEdicion').modal('hide');


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });




        });



        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });



    }


}

window.addEventListener('load', () => {

    wabout.init();

    wabout.accionesBotones();

});



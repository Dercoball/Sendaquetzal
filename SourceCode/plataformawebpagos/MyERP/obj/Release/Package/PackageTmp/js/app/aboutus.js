'use strict';
let pagina = '0';

const aboutus = {


    init: () => {

        //console.log('Start');
      
        aboutus.cargarItems();

    },

    cargarItems: () => {

        let params = {};
        params.path = "connbd";
        params.idUsuario = sessionStorage.getItem("idusuario");
        params.idTabla = "1";
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../pages/web/WAboutUS.aspx/LoadContentPublic",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;

                $('#loadItems').html(data);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });






    },



}

window.addEventListener('load', () => {

    aboutus.init();

});



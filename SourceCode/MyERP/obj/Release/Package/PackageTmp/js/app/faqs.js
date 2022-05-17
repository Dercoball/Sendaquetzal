'use strict';
let pagina = '0';


const faq = {


    init: () => {

        console.log('Start');
      
        faq.cargarItems();

    },

    cargarItems: () => {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = sessionStorage.getItem("idusuario");
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../pages/web/WFAQ.aspx/GetListaItemsPublic",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;
                let html = '';
                data.forEach((item, index) => {

                    //console.log(item, index);

                    html += `
                            <div>
                                <div style="margin-top: 20px;">
                                <strong>
                                ${item.Pregunta}
                                </strong>
                            </div>
                            <div>
                                ${item.Respuesta}
                            </div>
                    `;

                });

                $('#loadItems').html(html);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });






    },



}

window.addEventListener('load', () => {

    faq.init();

});



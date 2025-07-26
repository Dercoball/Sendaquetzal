'use strict';
let pagina = '0';


const faq = {


    init: () => {

        console.log('Start');

        faq.cargarItems();

    },


    cargarItems: () => {

        let params = {};
        params.path = "connbd";
        params.idUsuario = sessionStorage.getItem("idusuario");
        params.idTabla = "1";
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../pages/web/WTutoriales.aspx/GetListaItemsPublic",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                let data = msg.d;
                let html = '';
                data.forEach((item, index) => {


                    html += `
                          <div style="text-align: center">
                                <div style="margin-top: 20px; text-align: center">
                                <strong>
                                ${item.Titulo}
                                </strong>
                            </div>
                            <a href="${item.UrlVideo}">
                                ${item.UrlVideo}
                            </a>
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



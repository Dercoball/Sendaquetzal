'use strict';
let date = new Date();
let descargas = "CALENDARIO_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '58';




const calendar = {

    

    init: () => {


        calendar.loadContent();
       

    },

    loadContent() {

        let params = {};
        params.path = window.location.hostname;
        console.log(params.idUsuario = document.getElementById('txtIdUsuario').value);
        
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/Calendar.aspx/GetListaItemsFechas",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;

                //  si no tiene permisos
                if (data == null) {
                    window.location = "../../pages/Index.aspx";
                }

                let listFechas = document.getElementById("listaFechas");

                data.forEach((item) => {
                    let li = document.createElement("li");
                    li.classList.add("border");
                    li.innerText = item.Nombre;
                    listFechas.appendChild(li);

                })
                



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });

    },





    accionesBotones: () => {

      

    }


}

window.addEventListener('load', () => {

    calendar.init();

    calendar.accionesBotones();

});



'use strict';
let date = new Date();
let descargas = "Dashboard_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '80';


const dashBoard = {


    init: () => {

        //console.log('init...');

        dashBoard.cargarItems();
        //dashBoard.cargarOtsPorHacer();
        //dashBoard.cargarTotales();
        //dashBoard.cargarItemsColaboraciones();
        dashBoard.obtenerFechaHoraServidor();

        dashBoard.scrollTop = 100;
        dashBoard.incremento = 100;


    },


    obtenerFechaHoraServidor: (idControl) => {
        //console.log(`fecha ...`);

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

                //console.log(`fecha = ${JSON.stringify(fecha)}`);

                $('#fechaHora').html(`<h1 class="texto-hora">${fecha.Hora} Hrs</h1>`);
                //$('#' + idControl).prop('fecha', fecha.Fecha);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    },

    

    cargarItems: () => {

        dashBoard.cargarOtsPorHacer();
           
    },

    cargarOtsPorHacer: () => {

        $('#itemsOtsPorHacer').empty().append(`
            <img src = '../img/loading.gif' class='img img-fluid'/>
        `);

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");

        let url = '../pages/MantenimientoDashboard2.aspx/GetListaOtsPorHacer';

        utils.postData(url, parametros)
            .then(data => {

                data = data.d;
                let html = '';


                for (let i = 0; i < data.length; i++) {
                    let item = data[i];
                    let style = "bg-light";
                    let border = "border: solid 1px gray;";

                    if ((i + 1) % 3 === 0) {
                        //style = "bg-secondary";
                        border = "border: solid 1px #2b90d9;";
                    }

                    html += `

                    <div class="col-md-3 h-100">
                        <div class="card ${style} mb-3" style="width: 99%!important; ${border}">
                            <div class="card-header" >
                                                    OT<strong>${item.IdRequisicion}</strong> - ${item.NumeroEconomico}
                            </div>
                            <div class="card-body">
                                <div><small>${item.Descripcion}</small></div>
                                <div class="card-title">${item.NombreUbicacion}</div>
                         
                            </div>
                        </div> 
                    </div> `;

                }

                $('#contenedorItemsPorHacer').css('height', `${$('#contenedorTrabajosEnAtencion').height()}`);

                $('#itemsOtsPorHacer').empty().append(html).promise().done(() => {


                });


                setInterval(() => {

                    $("#contenedorItemsPorHacer").animate({ scrollTop: dashBoard.scrollTop });
                    dashBoard.scrollTop += dashBoard.incremento;

                    let itemsOtsPorHacerHeight = $('#itemsOtsPorHacer').height();

                    console.log(`scrollTop = ${dashBoard.scrollTop}`);
                    console.log(`itemsOtsPorHacer height ${itemsOtsPorHacerHeight}`);
                    console.log(`incremento ${dashBoard.incremento}`);

                    if (dashBoard.scrollTop >= itemsOtsPorHacerHeight) {
                        dashBoard.incremento = 0;
                        console.log(`Reset 0`);
                    }

                }, 5000);





            });



    },


    accionesBotones: () => {

    }


}

window.addEventListener('load', () => {

    dashBoard.init();


});


setInterval(() => {

    //console.log('Actualizando...');
    window.location.reload();
    //dashBoard.cargarItems();
    //dashBoard.cargarOtsPorHacer();
    //dashBoard.cargarTotales();


}, 1000 * 60 * 5);




setInterval(() => {

    //console.log('Actualizando...hora');
    dashBoard.obtenerFechaHoraServidor();

}, 1000 * 60 * 1);
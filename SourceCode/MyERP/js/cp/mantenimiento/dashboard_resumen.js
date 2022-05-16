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

    cargarItemsColaboraciones: () => {

        $('#trabajosEnColaboracion').empty().append(`
            <img src = '../img/loading.gif' class='img img-fluid'/>
        `);


        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");

        let url = '../pages/MantenimientoDashboard.aspx/GetItemsColaboracion';

        utils.postData(url, parametros)
            .then(data => {

                data = data.d;
                let html = '';
                //console.log(`data = ${JSON.stringify(data)}`);

                for (var i = 0; i < data.length; i++) {
                    var item = data[i];


                    let ot = item.IdRequisicion == 0 ?
                        `
                        <div class="progress">
                            <div class="progress-bar bg-danger" role="progressbar" style="width: 100%"
                            aria-valuenow="10" aria-valuemin="0" aria-valuemax="100">
                                Sin asignación
                            </div>
                        </div>
                        `
                        : `<p><a href="MantenimientoListadoFallas.aspx?id=${item.IdRequisicion}" target="blank" 
                                    class="btn btn-outline-primary">OT: ${item.IdRequisicion}</a></p>

                                 <div class="progress">
                                    <div class="progress-bar bg-info" role="progressbar" style="width: 100%"
                                        aria-valuenow="10" aria-valuemin="0" aria-valuemax="100">                                        
                                    </div>
                                </div>

                            `;

                    html += `


                            <div class="card mb-1 col-md-4">
                              <div class="card-body">
                                <h2 class="card-title">${item.NombreUsuario}</h2>
                                
                                ${ot}

                               

                              </div>
                            </div>

                           

						`;

                }

                $('#trabajosEnColaboracion').empty().append(html);


            });



    },


    cargarItems: () => {

        $('#trabajosEnAtencion').empty().append(`
            <img src = '../img/loading.gif' class='img img-fluid'/>
        `);


        let parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");

        let url = '../pages/MantenimientoDashboard.aspx/GetItemsEnAtencion';

        utils.postData(url, parametros)
            .then(data => {

                data = data.d;
                let html = '';

                for (var i = 0; i < data.length; i++) {
                    var item = data[i];
                    //console.log(`data = ${item.Tipo}`);

                    let ot = '';
                    if (item.Tipo === 'asignacion') {

                        ot = `

                                <p><a href="MantenimientoListadoFallas.aspx?id=${item.IdRequisicion}" target="blank" 
                                    class="btn btn-outline-primary">OT: ${item.IdRequisicion}</a></p>

                                 <div class="progress">
                                    <div class="progress-bar bg-success" role="progressbar" style="width: 100%"
                                        aria-valuenow="10" aria-valuemin="0" aria-valuemax="100">
                                        Asignación
                                    </div>
                                </div>

                            `;


                    }

                    if (item.Tipo === 'colaboracion') {

                        ot = `

                                <p><a href="MantenimientoListadoFallas.aspx?id=${item.IdRequisicion}" target="blank" 
                                    class="btn btn-outline-primary">OT: ${item.IdRequisicion}</a></p>

                                 <div class="progress">
                                    <div class="progress-bar bg-info" role="progressbar" style="width: 100%"
                                        aria-valuenow="10" aria-valuemin="0" aria-valuemax="100">
                                        Colaboración
                                    </div>
                                </div>

                            `;

                    }

                    if (item.Tipo === 'sinasignacion') {

                        ot = `

                            <div class="progress">
                                <div class="progress-bar bg-danger" role="progressbar" style="width: 100%"
                                aria-valuenow="10" aria-valuemin="0" aria-valuemax="100">
                                    Sin asignación
                                </div>
                            </div>

                            `;

                    }

                    //let foto = '';
                    //let isMobile = utils.isMobile();
                    //if (!isMobile) {
                    //    foto = `
                    //        <div class="row">
                    //            <div class="ml-auto mr-auto">
                    //                <img src="../img/user.png" class="img img-thumbnail" data-idusuario= ${item.IdUsuario}
                    //                    data-id="img_${item.IdUsuario}" id="img_${item.IdUsuario}" >
                    //                    </div>
                    //        </div>
                    //    `;

                    //}


                    html += `


                            <div class="card mb-1 col-md-4">
                              <div class="card-body">

                                    <h1 class="card-title text-center">${item.NombreUsuario}</h1>

                                <div class="text-muted">${item.NombreEquipo}</div>
                                <div class="text-muted"><strong>${item.NumeroEconomico}</strong></div>
                                <div class="text-muted">${item.NombreUbicacion}</div>

                                ${ot}

                               

                              </div>
                            </div>

                           

						`;

                }

                $('#trabajosEnAtencion').empty().append(html);

                //$(`.fotos`).each(function (f) {
                //    //console.log('cargar foto');

                //    let idUsuario =  $(this).attr('data-idusuario');
                //    let idImg = $(this).attr('data-id');

                //    //console.log('idImg = ' + idImg);

                //    var parametros = new Object();
                //    parametros.path = window.location.hostname;
                //    parametros.idUsuario = idUsuario;
                //    parametros = JSON.stringify(parametros);
                //    $.ajax({
                //        type: "POST",
                //        url: "../pages/Empleados.aspx/GetFotoByIdUsuario",
                //        data: parametros,
                //        contentType: "application/json; charset=utf-8",
                //        dataType: "json",
                //        async: true,
                //        success: function (foto) {

                //            let strFoto = foto.d;
                //            if (strFoto != '') {
                //                $(`#${idImg}`).attr('src', `data:image/jpg;base64,${strFoto}`);
                //            }

                //        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                //            console.log(textStatus + ": " + XMLHttpRequest.responseText);
                //        }

                //    });

                //});

                $('#toggle-btn').trigger('click');
                
                dashBoard.cargarOtsPorHacer();
            });



    },

    cargarOtsPorHacer: () => {

        $('#itemsOtsPorHacer').empty().append(`
            <img src = '../img/loading.gif' class='img img-fluid'/>
        `);

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");

        let url = '../pages/MantenimientoDashboard.aspx/GetListaOtsPorHacer';

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

                    <div class="col-md-6 h-100">
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


    cargarTotales: () => {

        $('#itemsTotales').empty().append(`
            <img src = '../img/loading.gif' class='img img-fluid'/>
        `);

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");

        let url = '../pages/MantenimientoDashboard.aspx/GetItemsTotalMensual';

        utils.postData(url, parametros)
            .then(data => {

                data = data.d;
                let html = '';
                //console.log(`data = ${JSON.stringify(data)}`);

                for (var i = 0; i < data.length; i++) {
                    var item = data[i];

                    html += `
                            <div class="container">
                                <div class="d-flex justify-content-between">
                                    <div class="left-col d-flex">
                                        <div class="text-info mr-2"><i class="fa fa-user"></i></div>
                                        <h2 class="card-title">
                                            ${item.NombreUsuario}                                        
                                        </h2>
                                    </div>
                                    <div class="right-col">
                                        <h1 class="text-success">${item.TotalRequisiciones}</h1>
                                    </div>
                                </div>
                            </div>

						`;

                }

                $('#itemsTotales').empty().append(html);


            });



    },

    cargarFactorEficiencia: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");

        let url = '../pages/MantenimientoDashboard.aspx/GetFactorEficiencia';

        utils.postData(url, parametros)
            .then(data => {

                data = data.d;
                let html = '';
                //console.log(`data = ${JSON.stringify(data)}`);


                for (var i = 0; i < data.length; i++) {
                    var item = data[i];

                    let factor = (Number(item.Factor) > 1.01)
                        ? `<p class="texto-rojo">${number_format(item.Factor, 3, '')}</p>`
                        : `<p>${number_format(item.Factor, 3, '')}</p>`;

                    html += `

                            <li class="d-flex justify-content-between">
                                <div class="left-col d-flex">
                                    <div class="icon"><i class="fa fa-user"></i></div>
                                    <div class="title">
                                        <strong data-id="${item.Id_Usuario}">${item.Nombre}</strong>
                                        `
                        + factor +
                        `
                                    </div>
                                </div>
                            </li>

						`;

                }

                $('#itemsFactor').empty().append(html);


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
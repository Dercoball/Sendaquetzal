'use strict';
let date = new Date();
let descargas = "CALENDARIO_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '58';




const calendar = {

    calendarControl: null,

    init: () => {


        calendar.setCalendar();


    },


    setCalendar: () => {

        let calendarEl = document.getElementById('calendar');
        calendar.calendarControl = new FullCalendar.Calendar(calendarEl, {
            locale: 'es',
            initialView: 'dayGridMonth'

        });
        calendar.calendarControl.render();


        const month = calendar.calendarControl.getDate().getMonth(); // ene = 0
        const year = calendar.calendarControl.getDate().getFullYear();
        console.log('month ' + month);
        console.log('year ' + year);
        calendar.getEventsByMonth(month, year);

        $('.fc-next-button').on('click', (e) => {

            console.log('click: ');


            const month = calendar.calendarControl.getDate().getMonth(); // ene = 0
            const year = calendar.calendarControl.getDate().getFullYear();
            console.log('month ' + month);
            console.log('year ' + year);
            calendar.getEventsByMonth(month, year);

        });

        $('.fc-prev-button').on('click', (e) => {

            console.log('click: ');


            const month = calendar.calendarControl.getDate().getMonth(); // ene = 0
            const year = calendar.calendarControl.getDate().getFullYear();
            console.log('month ' + month);
            console.log('year ' + year);
            calendar.getEventsByMonth(month, year);

        });


    },


    getEventsByMonth: (month, year) => {


        let params = {};
        params.path = window.location.hostname;
        params.month = month;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/Calendar.aspx/GetListaItemsFechasByMonth",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;

                let listFechas = document.getElementById("listaFechas");
                listFechas.innerHTML = '';


                //  Quitar eventos limpiar
                let currentEvents = calendar.calendarControl.getEvents();
                currentEvents.forEach((item) => {
                    item.remove();
                });

                data.forEach((item) => {

                    //  html
                    console.log(` Fecha 1 ${JSON.stringify(item.Fecha)}`);
                    console.log(`${JSON.stringify(item.FechaFinal)}`);

                    let li = document.createElement("li");
                    li.classList.add("border");
                    let fechaParts = item.FechaLarga.split(' ')[0] + " " + item.FechaLarga.split(' ')[1];

                    let icono = "<i class='fa fa-th-list mr-1'></i>";
                    if (item.Tipo === 'paro') {
                        icono = "<i class='fa fa-stop mr-1'></i>";
                    }
                    if (item.Nombre.includes('umplea')) {
                        icono = "<i class='fa fa-birthday-cake mr-1'></i>";

                        item.Fecha = year + "-" + item.Fecha.split('-')[1] + "-" + item.Fecha.split('-')[2];

                        console.log(` Fecha 2 = ${JSON.stringify(item.Fecha)}`);
                    }

                    li.innerHTML = icono + " <strong>" + fechaParts + "</strong> <div>" + item.Nombre + "</div>";
                    listFechas.appendChild(li);


                    //  nuevos eventos para el calendar para el mes seleccionado
                    let event =
                    {
                        title: item.Nombre,
                        start: item.Fecha,
                        end: item.FechaFinal,
                        color: item.Tipo === 'paro' ? 'red' : 'blue'
                    };

                    calendar.calendarControl.addEvent(event);

                });


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



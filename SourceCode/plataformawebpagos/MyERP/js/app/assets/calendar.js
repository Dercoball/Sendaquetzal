'use strict';
let date = new Date();
let descargas = "CALENDARIO_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '58';




const calendar = {

    calendarControl: null,

    init: () => {

        moment.locale('es-mx');
        calendar.setCalendar();
        calendar.loadComboPlaza();

        $('#cmbPlaza').change(function () {
            const month = calendar.calendarControl.getDate().getMonth(); // ene = 0
            const year = calendar.calendarControl.getDate().getFullYear();
            calendar.getEventsByMonth(month, year);
        });
    },

    loadComboPlaza: () => {

        var params = {};
        params.path = "connbd";
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Employees.aspx/GetListaItemsPlazas",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                let items = msg.d;
                let selectEl = document.getElementById('cmbPlaza');
                //remueve las opciones del combo
                document.querySelectorAll('select[name="cmbPlaza"] option').forEach(option => option.remove());
                selectEl.add(new Option("Todos", 0, true, true));

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];
                    const option = new Option(item.Nombre, item.IdPlaza, false, false);
                    selectEl.add(option);
                }

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    setCalendar: () => {

        let calendarEl = document.getElementById('calendar');
        calendar.calendarControl = new FullCalendar.Calendar(calendarEl, {
            locale: 'es',
            defaultView: 'month',
            headerToolbar: {
                start: 'prev',
                center: 'title',
                end: 'next'
            },

        });
        calendar.calendarControl.render();


        const month = calendar.calendarControl.getDate().getMonth(); // ene = 0
        const year = calendar.calendarControl.getDate().getFullYear();
        calendar.getEventsByMonth(month, year);

        $('.fc-next-button').on('click', (e) => {
            const month = calendar.calendarControl.getDate().getMonth(); // ene = 0
            const year = calendar.calendarControl.getDate().getFullYear();
            calendar.getEventsByMonth(month, year);

        });

        $('.fc-prev-button').on('click', (e) => {
            const month = calendar.calendarControl.getDate().getMonth(); // ene = 0
            const year = calendar.calendarControl.getDate().getFullYear();
            calendar.getEventsByMonth(month, year);
        });
    },


    getEventsByMonth: (month, year) => {
        let params = {};
        params.path = "connbd";
        params.month = month;
        params.year = year;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.plaza = parseInt(document.getElementById('cmbPlaza').value || 0);
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

                let containerFechas = document.getElementById("container-eventos");
                containerFechas.innerHTML = '';


                //  Quitar eventos limpiar
                let currentEvents = calendar.calendarControl.getEvents();
                currentEvents.forEach((item) => {
                    item.remove();
                });

                data.forEach((item) => {
                    let fechaInicial = moment(item.Fecha);
                    let fechaFinal = moment(item.FechaFinal);
                    let tipoEvento = item.Tipo === 'paro' ? 'paro' : (item.EsLaboral ? 'especial' : 'feriado');
                    let titulo = tipoEvento == 'paro' ? 'Semana de paro' : (tipoEvento == 'feriado' ? 'Día feriado' : item.Nombre);
                    let bg = item.Tipo === 'paro' ? 'rgba(255, 0, 0, 0.7)' : (item.EsLaboral ? 'rgba(0, 255, 0, 0.7)' : 'rgba(255, 165, 0, 0.7)');
                    let div = document.createElement("div");
                    div.classList.add("row", "mb-3");
                   

                    div.innerHTML = `
                                        <div class="col-auto">
                                            <div class="d-flex flex-column px-3 py-2" style="background-color: ${bg};">
                                                <div class="text-center text-white font-weight-bold">${fechaInicial.format('DD')}</div>
                                                <div class="text-uppercase text-center text-white font-weight-bold" style="font-size: 0.6rem;">${fechaInicial.format('MMM')}</div>
                                            </div>
                                        </div>
                                        <div class="col">
                                            <h4 class="m-0">${titulo}</h4>
                                            <span class="text-black-50">${item.Nombre}</span>
                                        </div>
                                    `;
                    containerFechas.appendChild(div);


                    //  nuevos eventos para el calendar para el mes seleccionado
                    let event =
                    {
                        title: '',
                        start: fechaInicial.format('YYYY-MM-DD'),
                        end: fechaFinal.add(1, 'days').format('YYYY-MM-DD'),
                        allDay: true,
                        display: 'background',
                        backgroundColor: item.Tipo === 'paro' ? 'rgba(255, 0, 0, 0.3)' : (item.EsLaboral ? 'rgba(0, 255, 0, 0.3)' : 'rgba(255, 165, 0, 0.3)'),
                        borderColor: item.Tipo === 'paro' ? 'red' : (item.EsLaboral ? 'green' : 'orange'),
                        classNames: [item.Tipo === 'paro' ? 'ev-paro' : (item.EsLaboral ? 'ev-normal' : 'ev-feriado')],
                        textColor: 'white'
                    };

                    console.log(event);

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



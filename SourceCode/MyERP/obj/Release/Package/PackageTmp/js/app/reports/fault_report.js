'use strict';
let date = new Date();
let descargas = "Reportes" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '19';


const employeeEvaluation = {


    init: () => {


        employeeEvaluation.idSolicitud = -1;
        employeeEvaluation.idEmpleado = -1;
        employeeEvaluation.idComision = -1;
        employeeEvaluation.NombreComision = '';
        employeeEvaluation.NombreEmpleado = '';

        employeeEvaluation.loadComboPlaza();
        employeeEvaluation.loadComboEmployeesByPosicion(utils.POSICION_PROMOTOR, '#comboPromotor');
        employeeEvaluation.loadComboEmployeesByPosicion(utils.POSICION_EJECUTIVO, '#comboEjecutivo');
        employeeEvaluation.loadComboEmployeesByPosicion(utils.POSICION_SUPERVISOR, '#comboSupervisor');

        //employeeEvaluation.cargarItems();

        $('.secciones').hide();
        $('#panelTabla').show();

        employeeEvaluation.calificacionTotal = 0;
    },

    cargarItems: () => {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params.idPlaza = document.getElementById('comboPlaza').value;
        params.idPromotor = document.getElementById('comboPromotor').value;
        params.idSupervisor = document.getElementById('comboSupervisor').value;
        params.idEjecutivo = document.getElementById('comboEjecutivo').value;
        params.fechaInicial = employeeEvaluation.fechaInicial;
        params.fechaFinal = employeeEvaluation.fechaFinal;
        params.idStatus = status;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Commissions/EmployeeEvaluation.aspx/GetListaItems",
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

                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: data,
                    columns: [
                        { data: 'NombreCompleto' },
                        { data: 'NombreCompletoSupervisor' },
                        { data: 'NombreCompletoEjecutivo' },
                        { data: 'NombrePlaza' },
                        { data: 'FechaIngresoMx' },
                        { data: 'NombreComision' },
                        { data: 'nivelNomision.PorcentajeStr' },
                        { data: 'Accion' }


                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        }
                    ],
                    dom: 'fBrtipl',
                    buttons: [
                        {
                            extend: 'csvHtml5',
                            title: descargas,
                            text: '&nbsp;Csv', className: 'csvbtn'
                        }
                    ]


                });


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },


    loadComboEmployeesByPosicion: (idTipoEmpleado, control) => {

        var params = {};
        params.path = window.location.hostname;
        params.idTipoEmpleado = idTipoEmpleado;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Employees.aspx/GetListaItemsEmpleadoByPosicion",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="-1">Todos</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdEmpleado}' > ${item.Nombre}</option > `;

                }

                $(`${control}`).html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },

    loadComboPlaza: () => {

        var params = {};
        params.path = window.location.hostname;
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
                let opcion = '<option value="-1">Todos</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdPlaza}' > ${item.Nombre}</option > `;

                }

                $('#comboPlaza').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    open(idComision, nombreComision, nombreEmpleado, idEmpleado) {

        console.log(`idComision ${idComision}`);
        employeeEvaluation.idComision = idComision;
        employeeEvaluation.nombreComision = nombreComision;
        employeeEvaluation.nombreEmpleado = nombreEmpleado;
        employeeEvaluation.idEmpleado = idEmpleado;

        employeeEvaluation.calificacionTotal = 0;

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idComision = idComision;
        params.idEmpleado = idEmpleado;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Commissions/EmployeeEvaluation.aspx/GetItemsRulesByCommissionAndEmployee",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;
                console.table(data.table);

                //  si no tiene permisos
                if (data == null) {
                    window.location = "../../pages/Index.aspx";
                }


                $('#tableReglas tbody').empty().append(data.Table);
                employeeEvaluation.calificacionTotal = Number(data.totalEvaluacionCompletada);

                //if (Number(employeeEvaluation.calificacionTotal) > 0) {
                //    $('#spanTotalPonderacionObtenida').text(employeeEvaluation.calificacionTotal + '%');
                //}


                $('#nombreComision').html(employeeEvaluation.nombreComision);
                $('#nombreEmpleado').html(employeeEvaluation.nombreEmpleado);

                $('.secciones').hide();
                $('#panelForm').show();

                $(`.checks`).on('change', (e) => {
                    e.preventDefault();

                    let id = e.currentTarget.id
                    console.log(`${id}`);
                    console.log('check');

                    let checked = $(`#${id}`).prop('checked');
                    let value = $(`#${id}`).attr('data-value');

                    //console.log('checked ' + checked);
                    //console.log('value ' + value);

                    if (checked) {
                        employeeEvaluation.calificacionTotal += Number(value);
                    } else {
                        employeeEvaluation.calificacionTotal -= Number(value);
                    }
                    if (Number(employeeEvaluation.calificacionTotal) > 0) {
                        $('#spanTotalPonderacionObtenida').text(employeeEvaluation.calificacionTotal + '%');
                    }

                });


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },



    accionesBotones: () => {




        
        $('#btnReporteFalla').on('click', (e) => {
            e.preventDefault();

            console.log('btnReporteFalla');

            var element = document.getElementById('divReporteFalla');
            html2pdf(element);

        });

    }


}

window.addEventListener('load', () => {

    employeeEvaluation.init();

    employeeEvaluation.accionesBotones();

});



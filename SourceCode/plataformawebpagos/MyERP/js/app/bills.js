'use strict';
let date = new Date();
let descargas = "RegistrodeGastos_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '20';




const billsRegister = {


    init: () => {

        $('.secciones').hide();
        $('#panelTabla').show();

        billsRegister.idReglaSeleccionada = -1;
        billsRegister.accion = '';
        billsRegister.idComision = -1;
        billsRegister.NombreComision = '';

        billsRegister.loadItems();


    },

    fechaHoy() {
        let today = new Date();

        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        $('#txtFecha').val(`${dayMonth}/${month}/${today.getFullYear()}`);

    },

    loadItems() {

        let params = {};
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Bills.aspx/GetItems",
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
                        { data: 'Concepto' },
                        { data: 'Fecha' },
                        { data: 'MontoFormateadoMx' },
                        { data: 'Accion' }


                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        },
                    ],
                    dom: 'frBtipl',
                    buttons: [
                        {
                            extend: 'csvHtml5',
                            title: descargas,
                            text: '&nbsp;Csv', className: 'csvbtn'
                        },
                    ]

                });


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });



    },




    nuevo: () => {


        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloForm').text('Nuevo');


        $('.secciones').hide();
        $('#panelForm').show();

        billsRegister.accion = "nuevo";
        billsRegister.idSeleccionado = -1;
        billsRegister.fechaHoy();

        $('.deshabilitable').prop('disabled', false);




    },



    accionesBotones: () => {

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            billsRegister.nuevo();

        });


        $('#btnGuardar').on('click', function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                let item = {};
                item.IdGasto = billsRegister.idReglaSeleccionada;
                item.Concepto = $('#txtConcepto').val();
                item.Monto = $('#txtMonto').val();
                item.IdUsuario = document.getElementById('txtIdUsuario').value;
                item.IdEmpleado = document.getElementById('txtIdEmpleado').value;


                let params = {};
                params.path = "connbd";
                params.item = item;
                params.accion = billsRegister.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Bills.aspx/Save",
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        let resultado = parseInt(msg.d);

                        if (resultado > 0) {

                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                            billsRegister.loadItems();

                            $('.secciones').hide();
                            $('#panelTabla').show();

                        } else {

                            utils.toast(mensajesAlertas.errorGuardar, 'fail');

                        }

                        $('#panelEdicion').modal('hide');


                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });


            }

        });



        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('.secciones').hide();
            $('#panelTabla').show();

        });


    }





}

window.addEventListener('load', () => {

    billsRegister.init();

    billsRegister.accionesBotones();

});



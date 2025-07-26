'use strict';
let dateBills = new Date();
let descargasBills = "RegistrodeGastos_" + dateBills.getFullYear() + "_" + dateBills.getMonth() + "_" + dateBills.getUTCDay() + "_" + dateBills.getMilliseconds();
let paginaBills = '20';




const billsRegister = {


    init: () => {

        $('.secciones').hide();
        $('#panelTablaBills').show();

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

        $('#txtFechaBills').val(`${dayMonth}/${month}/${today.getFullYear()}`);

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

                let table = $('#tableBills').DataTable({
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
                            title: descargasBills,
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


        $('#frmBills')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloFormBills').text('Nuevo');


        $('.secciones').hide();
        $('#panelFormBills').show();

        billsRegister.accion = "nuevo";
        billsRegister.idSeleccionado = -1;
        billsRegister.fechaHoy();

        $('.deshabilitable').prop('disabled', false);




    },



    accionesBotones: () => {

        $('#btnNuevoBills').on('click', (e) => {
            e.preventDefault();

            billsRegister.nuevo();

        });


        $('#btnGuardarBills').on('click', function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frmBills"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                let item = {};
                item.IdGasto = billsRegister.idReglaSeleccionada;
                item.Concepto = $('#txtConceptoBills').val();
                item.Monto = $('#txtMontoBills').val();
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
                            $('#panelTablaBills').show();

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



        $('#btnGuardarBills').on('click', (e) => {
            e.preventDefault();

            $('.secciones').hide();
            $('#panelTablaBills').show();

        });

        $('#btnEliminarAceptarBills').on('click', (e) => {

            let params = {};
            params.path = "connbd";
            params.id = plaza.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Assets/Bills.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        plaza.loadContent();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });


    }





}

window.addEventListener('load', () => {

    billsRegister.init();

    billsRegister.accionesBotones();

});



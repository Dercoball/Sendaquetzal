'use strict';




const retirement = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();
        $('#panelFormRetiro').hide();

        retirement.idSeleccionado = -1;
        retirement.idInversionista = -1;
        retirement.accion = '';

        $('#txtFecha').val(retirement.fechaHoy());

        console.log(retirement.fechaHoy());
    },


    addRetiro: (id, idInversion, monto) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#frmRetiro')[0].reset();

        $('#txtFecha').val(retirement.fechaHoy());
        $('#txtMonto').val(monto);


        retirement.getData(id);


        retirement.idInversionista = id;
        retirement.idSeleccionado = idInversion;

    },




    getData: (id) => {


        let params = {};
        params.path = "connbd";
        params.id = id;
        console.log(id);
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Investors/Investments.aspx/GetDataInvestor",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let item = msg.d;
                

                $('#txtNombre').val(item.Nombre);
                $('#txtRazonSocial').val(item.RazonSocial);


                $('#panelTabla').hide();
                $('#panelFormRetiro').show();


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },

    fechaHoy() {
        let today = new Date();

        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        return `${today.getFullYear()}-${month}-${dayMonth}`;

    },



    accionesBotones: () => {

        
        $('#btnGuardarRetiro').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frmRetiro"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                //Objeto con los valores a enviar
                let item = {};
                item.fecha = $('#txtFecha').val();
                item.IdInversion = retirement.idSeleccionado;
                item.Monto = $('#txtMonto').val();
                item.idInversionista = retirement.idInversionista;


                let params = {};
                params.path = "connbd";
                params.item = item;
                params.accion = retirement.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Investors/Investments.aspx/SaveRetiro",
                    
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        var valores = msg.d;

                        if (parseInt(valores.CodigoError) == 0) {

                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                            $('.file-comprobanteretiro').each(function (documento) {

                                let file;
                                if (file = this.files[0]) {

                                    console.log("guardar comprobante retiro");

                                    utils.sendFileEmployee(file, 'comprobanteRetiro', valores.IdItem, 0, "comprobanteRetiro");

                                }

                            });


                            $('#panelTabla').show();
                            $('#panelFormRetiro').hide();

                            asset.loadContent();


                        } else {

                            utils.toast(mensajesAlertas.errorGuardar, 'fail');


                        }

                        


                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });


            }

        });



        $('#btnCancelarRetiro').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelFormRetiro').hide();

        });


        $('#btnEliminarAceptar').on('click', (e) => {

            let params = {};
            params.path = "connbd";
            params.id = retirement.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Investors/Investors.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        retirement.loadContent();

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

    retirement.init();

    retirement.accionesBotones();

});



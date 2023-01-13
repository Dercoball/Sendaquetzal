'use strict';
let dateAssets = new Date();
let descargasAssets = "ACTIVOS_" + dateAssets.getFullYear() + "_" + dateAssets.getMonth() + "_" + dateAssets.getUTCDay() + "_" + dateAssets.getMilliseconds();
let paginaAssets = '52';




const asset = {

    

    init: () => {

        $('#panelTablaAssets').show();
        $('#panelFormAssets').hide();

        asset.idSeleccionado = -1;
        asset.accion = '';

        asset.loadContent();
        asset.loadComboEmpleado();
        asset.loadComboTipo();

    },

    loadContent() {

        let params = {};
        params.path = window.location.hostname;
        console.log(params.idUsuario = document.getElementById('txtIdUsuario').value);
        
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/Assets.aspx/GetListaItems",
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

                let table = $('#tableAssets').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: data,
                    columns: [

                        { data: 'Categoria.Nombre' },
                        { data: 'Descripcion' },
                        { data: 'NumeroSerie' },
                        { data: 'Costo' },
                        { data: 'Comentarios' },
                        { data: 'Empleado.Nombre' },
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
                            extend: 'excelHtml5',
                            title: descargasAssets,
                            text: 'Xls', className: 'excelbtn'
                        },
                        {
                            extend: 'pdfHtml5',
                            title: descargasAssets,
                            text: 'Pdf', className: 'pdfbtn'
                        }
                    ]

                });


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });

    },


    delete: (id) => {

        asset.idSeleccionado = id;


        $('#mensajeEliminar').text(`Se eliminará el registro seleccionado (No. ${id}). ¿Desea continuar ?`);
        $('#panelEliminar').modal('show');

    },




    edit: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#frmAssets')[0].reset();

        let params = {};
        params.path = window.location.hostname;
        params.id = id;
        console.log(id);
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/Assets.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let item = msg.d;
                asset.idSeleccionado = item.IdActivo;

                $('#txtDescripcion').val(item.Descripcion);
                $('#comboCategoriaAssets').val(item.Categoria.Id);
                $('#comboEmpleadoAssets').val(item.Empleado.IdEmpleado);

                $('#txtNumeroSerie').val(item.NumeroSerie);
                $('#txtCostoAssets').val(item.Costo);
                $('#txtComentarios').val(item.Comentarios);



                $('#panelTablaAssets').hide();
                $('#panelFormAssets').show();


                asset.accion = "editar";
                $('#spnTituloFormAssets').text('Editar');
                $('.deshabilitable').prop('disabled', false);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },



   

    loadComboEmpleado: () => {

        var params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);
        console.log("PRUEBA COMBO", params);
        $.ajax({
            type: "POST",
            url: "../../pages/Assets/Assets.aspx/GetListaItemsEmpleados",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdEmpleado}' > ${item.Nombre}</option > `;

                }

                $('#comboEmpleadoAssets').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },

    loadComboTipo: () => {

        var params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        console.log("PRUEBA COMBO", params);
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/Assets.aspx/GetListaItemsCategorias",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.Id}' > ${item.Nombre}</option > `;

                }

                $('#comboCategoriaAssets').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    nuevo: () => {


        $('#frmAssets')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloFormAssets').text('Nuevo');




        $('#panelTablaAssets').hide();
        $('#panelFormAssets').show();
        asset.accion = "nuevo";
        asset.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);




    },



    accionesBotones: () => {

        $('#btnNuevoAssets').on('click', (e) => {
            e.preventDefault();

            asset.nuevo();

        });


        $('#btnGuardarAssets').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frmAssets"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                //  Objeto con los valores a enviar
                let item = {};

                item.IdActivo = asset.idSeleccionado;
                item.Descripcion = $('#txtDescripcion').val();
                item.IdCategoria = $('#comboCategoriaAssets').val();
                item.IdEmpleado = $('#comboEmpleadoAssets').val();
                item.NumeroSerie = $('#txtNumeroSerie').val();
                item.Costo = $('#txtCostoAssets').val();
                item.Comentarios = $('#txtComentarios').val();



                let params = {};
                params.path = window.location.hostname;
                params.item = item;
                params.accion = asset.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Assets/Assets.aspx/Save",
                    
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        let resultado = parseInt(msg.d);

                        if (resultado > 0) {

                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                            $('#panelTablaAssets').show();
                            $('#panelFormAssets').hide();

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

        $('#btnEliminarAceptar').on('click', (e) => {

            let params = {};
            params.path = window.location.hostname;
            params.id = asset.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Assets/Assets.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        asset.loadContent();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });




        $('#btnCancelarAssets').on('click', (e) => {
            e.preventDefault();

            $('#panelTablaAssets').show();
            $('#panelFormAssets').hide();

        });

    }


}

window.addEventListener('load', () => {

    asset.init();

    asset.accionesBotones();

});



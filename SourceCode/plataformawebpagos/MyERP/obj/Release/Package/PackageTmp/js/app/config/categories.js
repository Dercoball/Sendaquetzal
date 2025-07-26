'use strict';
let dateAssets = new Date();
let descargasAssets = "CATEGORIAS_" + dateAssets.getFullYear() + "_" + dateAssets.getMonth() + "_" + dateAssets.getUTCDay() + "_" + dateAssets.getMilliseconds();
let paginaAssets = '45';





const Category = {


    init: () => {

        $('#panelTablaAssets').show();
        $('#panelFormAssets').hide();

        Category.idSeleccionado = -1;
        Category.accion = '';

        Category.loadContent();


    },

    loadContent() {

        let params = {};
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Categories.aspx/GetListaItems",
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

                        { data: 'Id' },
                        { data: 'Nombre' },
                        { data: 'EsMaterialEntregaStr' },
                        { data: 'ActivoStr' },
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

        Category.idSeleccionado = id;


        $('#mensajeEliminarAssets').text(`Se eliminará el registro seleccionado (No. ${id}). ¿Desea continuar ?`);
        $('#panelEliminarAssets').modal('show');

    },




    edit: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();

        let params = {};
        params.path = "connbd";
        params.id = id;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Categories.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                Category.idSeleccionado = item.Id;

                $('#txtNombreAssets').val(item.Nombre);

                $('#chkActivoAssets').prop('checked', item.Activo === 1);
                $('#chkEsMaterialEntrega').prop('checked', item.EsMaterialEntrega === 1);

                $('#panelTablaAssets').hide();
                $('#panelFormAssets').show();


                Category.accion = "editar";
                $('#spnTituloFormAssets').text('Editar');
                $('.deshabilitable').prop('disabled', false);

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
        Category.accion = "nuevo";
        Category.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);




    },


    accionesBotones: () => {

        $('#btnNuevoAssets').on('click', (e) => {
            e.preventDefault();

            Category.nuevo();

        });


        $('#btnGuardarAssets').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frmAssets"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                let item = {};
                item.Id = Category.idSeleccionado;
                item.Nombre = $('#txtNombreAssets').val();
                item.Activo = $('#chkActivoAssets').prop('checked') ? 1 : 0;
                item.EsMaterialEntrega = $('#chkEsMaterialEntrega').prop('checked') ? 1 : 0;

                let params = {};
                params.path = "connbd";
                params.item = item;
                params.accion = Category.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Config/Categories.aspx/Save",
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

                            Category.loadContent();


                        } else {

                            utils.toast(mensajesAlertas.errorGuardar, 'fail');


                        }

                        $('#panelEdicionAssets').modal('hide');


                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });


            }

        });



        $('#btnCancelarAssets').on('click', (e) => {
            e.preventDefault();

            $('#panelTablaAssets').show();
            $('#panelFormAssets').hide();

        });


        $('#btnEliminarAceptarAssets').on('click', (e) => {

            let params = {};
            params.path = "connbd";
            params.id = Category.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Config/Categories.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        Category.loadContent();

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

    Category.init();

    Category.accionesBotones();

});



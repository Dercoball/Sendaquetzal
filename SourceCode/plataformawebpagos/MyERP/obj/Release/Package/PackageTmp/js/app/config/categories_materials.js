'use strict';
let dateMaterials = new Date();
let descargasMaterials = "CATEGORIAS_" + dateMaterials.getFullYear() + "_" + dateMaterials.getMonth() + "_" + dateMaterials.getUTCDay() + "_" + dateMaterials.getMilliseconds();
let paginaMaterials = '202';





const CategoryMaterial = {


    init: () => {

        $('#panelTablaMaterials').show();
        $('#panelFormMaterials').hide();

        CategoryMaterial.idSeleccionado = -1;
        CategoryMaterial.accion = '';

        CategoryMaterial.loadContent();


    },

    loadContent() {

        let params = {};
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/CategoriesMaterials.aspx/GetListaItems",
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

                let table = $('#tableMaterials').DataTable({
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
                            title: descargasMaterials,
                            text: 'Xls', className: 'excelbtn'
                        },
                        {
                            extend: 'pdfHtml5',
                            title: descargasMaterials,
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

        CategoryMaterial.idSeleccionado = id;


        $('#mensajeEliminarMaterials').text(`Se eliminará el registro seleccionado (No. ${id}). ¿Desea continuar ?`);
        $('#panelEliminarMaterials').modal('show');

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
            url: "../../pages/Config/CategoriesMaterials.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                CategoryMaterial.idSeleccionado = item.Id;

                $('#txtNombreMaterials').val(item.Nombre);

                $('#chkActivoMaterials').prop('checked', item.Activo === 1);
                $('#chkEsMaterialEntregaMaterials').prop('checked', item.EsMaterialEntrega === 1);

                $('#panelTablaMaterials').hide();
                $('#panelFormMaterials').show();


                CategoryMaterial.accion = "editar";
                $('#spnTituloFormMaterials').text('Editar');
                $('.deshabilitable').prop('disabled', false);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },


    nuevo: () => {


        $('#frmMaterials')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloFormMaterials').text('Nuevo');




        $('#panelTablaMaterials').hide();
        $('#panelFormMaterials').show();
        CategoryMaterial.accion = "nuevo";
        CategoryMaterial.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);




    },


    accionesBotones: () => {

        $('#btnNuevoMaterials').on('click', (e) => {
            e.preventDefault();

            CategoryMaterial.nuevo();

        });


        $('#btnGuardarMaterials').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frmMaterials"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                let item = {};
                item.Id = CategoryMaterial.idSeleccionado;
                item.Nombre = $('#txtNombreMaterials').val();
                item.Activo = $('#chkActivoMaterials').prop('checked') ? 1 : 0;
                item.EsMaterialEntrega = $('#chkEsMaterialEntregaMaterials').prop('checked') ? 1 : 0;

                let params = {};
                params.path = "connbd";
                params.item = item;
                params.accion = CategoryMaterial.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Config/CategoriesMaterials.aspx/Save",
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        let resultado = parseInt(msg.d);

                        if (resultado > 0) {

                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                            $('#panelTablaMaterials').show();
                            $('#panelFormMaterials').hide();

                            CategoryMaterial.loadContent();


                        } else {

                            utils.toast(mensajesAlertas.errorGuardar, 'fail');


                        }

                        $('#panelEdicionMaterials').modal('hide');


                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });


            }

        });



        $('#btnCancelarMaterials').on('click', (e) => {
            e.preventDefault();

            $('#panelTablaMaterials').show();
            $('#panelFormMaterials').hide();

        });


        $('#btnEliminarAceptarMaterials').on('click', (e) => {

            let params = {};
            params.path = "connbd";
            params.id = CategoryMaterial.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Config/CategoriesMaterials.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        CategoryMaterial.loadContent();

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

    CategoryMaterial.init();

    CategoryMaterial.accionesBotones();

});




const datosEquipo = {

    odometro: (id) => {

        let parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idEquipo = id;
        parametros = JSON.stringify(parametros);

        $.ajax({
            type: "POST",
            url: "../pages/PanelEquipos.aspx/GetUltimoOdometroReportado",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg_) {

                var odometro = msg_.d;

                $('#txtOdometro').val('');
                $('#txtFechaOdometro').val('');
                $('#comboUltimaUbicacion').val(null);

                if (odometro != null) {
                    $('#txtOdometro').val(odometro.odometro);
                    $('#txtFechaOdometro').val(odometro.fechaFormateadaMx);
                    $('#comboUltimaUbicacion').val(odometro.IdUbicacion);

                }

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    },


    cargarComboEstatus: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaEstatus",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdStatusProyecto + '">' + item.Nombre + '</option>';

                }

                $('#comboEstatus').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    

    cargarComboMarcas: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaMarcas",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.Id_Marca + '">' + item.Nombre + '</option>';

                }

                $('#comboMarca').empty().append(opcion);


                //var dataAdapter = new $.jqx.dataAdapter(items);


                //$("#comboMarcaProveedor").jqxDropDownList({
                //    source: dataAdapter, displayMember: "Nombre", valueMember: "Id_Marca", width: '250px',
                //    height: '20px', placeHolder: "Seleccione:", filterable: true, searchMode: 'containsignorecase',
                //    filterPlaceHolder: 'Buscar'
                //});
                //$("#comboMarcaProveedor").jqxDropDownList('clearSelection');



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    cargarComboModelos_: (idMarca, idModelo) => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idMarca = idMarca;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaModelos",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";
                opcion += '<option value=""></option>';

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.Id_Modelo + '">' + item.Nombre + '</option>';

                }

                $('#comboModelo').empty().append(opcion);

                if (idModelo !== 0) {
                    $('#comboModelo').val(idModelo);
                }
                //

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },

    cargarComboUnidadesMedida: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelEquipos.aspx/GetListaUnidadesMedidaOrometro",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdUnidad + '">' + item.Nombre + '</option>';

                }

                $('#comboUnidadMedidaOrometro').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    cargarComboUnidades: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoPanelSolicitudRefacciones.aspx/GetListaUnidades",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdUnidad + '">' + item.Nombre + '</option>';

                }

                $('#comboUnidad').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    cargarComboUbicaciones: (idUbicacion) => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoConsultaEquipos.aspx/GetListaUbicaciones",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdUbicacion + '">' + item.Nombre + '</option>';

                }

                $('#comboUbicacion').empty().append(opcion);
                $('#comboUltimaUbicacion').empty().append(opcion);

                if (idUbicacion != null && idUbicacion !== 0) {
                    $('#comboUbicacion').val(idUbicacion);
                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    },



    cargarComboModelos: (idMarca, idModelo) => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idMarca = idMarca;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaModelos",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";
                opcion += '<option value=""></option>';

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.Id_Modelo + '">' + item.Nombre + '</option>';

                }

                $('#comboModelo').empty().append(opcion);

                if (idModelo !== 0) {
                    $('#comboModelo').val(idModelo);
                }
                //

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },



}

const imagenes = {


    mostrarImagenes: (idDetalleSolicitud, tipo) => {

        //  Mostrar las imagenes
        //console.log(`${tipo}`);
        //console.log(`${idDetalleSolicitud}`);

        if (tipo == null || tipo === 1) {

            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.idDetalleSolicitud = idDetalleSolicitud;
            parametros.tipo = 1;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Combustibles_Solicitud.aspx/GetFoto",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (foto) {

                    let strFoto = foto.d;
                    if (strFoto != '') {
                        $('#img_1').attr('src', `data:image/jpg;base64,${strFoto}`);
                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        }

        if (tipo == null || tipo === 2) {
            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.idDetalleSolicitud = idDetalleSolicitud;
            parametros.tipo = 2;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Combustibles_Solicitud.aspx/GetFoto",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (foto) {

                    let strFoto = foto.d;
                    if (strFoto != '') {
                        $('#img_2').attr('src', `data:image/jpg;base64,${strFoto}`);
                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });
        }

        if (tipo == null || tipo === 3) {

            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.idDetalleSolicitud = idDetalleSolicitud;
            parametros.tipo = 3;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Combustibles_Solicitud.aspx/GetFoto",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (foto) {

                    let strFoto = foto.d;
                    if (strFoto != '') {
                        $('#img_3').attr('src', `data:image/jpg;base64,${strFoto}`);
                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        }


    },

    guardarImagenCombustible: (id, fecha, numeroImagen, file, tipo) => {

        imagenes.sendFileCombustible(file, numeroImagen, id, tipo, fecha, numeroImagen);

        setTimeout(function () {

            imagenes.mostrarImagenes(id, numeroImagen);

        }, 5000);

    },

    sendFileCombustible: (file, nombreArchivo, idItem, tipo, fechaSolicitud, numeroImagen) => {


        var files = file;

        var fileName = files.name;
        var extension = utils.getFileExtension(fileName);
        var idUsuario = sessionStorage.getItem("idusuario");

        //console.log('llego aqui');

        var formData = new FormData();
        formData.append('file', file);
        formData.append('id', idItem);//
        formData.append('pagina', window.location.pathname);//
        formData.append('path', window.location.hostname);//
        formData.append('extension', extension);//
        formData.append('descripcion', fileName);//
        formData.append('tipo', 'fotografiaSolicitudCombustible');//
        formData.append('idUsuario', idUsuario);//
        formData.append('nombreArchivo', numeroImagen);
        formData.append('fecha', fechaSolicitud);



        $.ajax({
            type: 'post',
            url: '../pages/fileUploader.ashx',
            data: formData,
            success: function (status) {
                if (status != 'error') {


                }
            },
            processData: false,
            contentType: false,
            error: function () {

            }
        });
    },


}


window.addEventListener('load', () => {


});

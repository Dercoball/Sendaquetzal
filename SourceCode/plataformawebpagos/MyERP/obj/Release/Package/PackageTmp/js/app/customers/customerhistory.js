let date = new Date();
let descargas = "HistorialCliente_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '14';


let solicitud = {
    idSolicitud: -1,
    idUsuario: -1,
    solicitud: {},
    arrayData: [],
    accion: '',
    idSeleccionado: -1,
    idDetalleSolicitud: -1,
    fecha: '',
    numero: -1
}


class SolicitudCombustible {

    constructor(Id_) {
        this.IdRegistroFalla = Id_;
        this.IdEquipo = null;
        this.FechaCreacion = null;

    }

}

const registroHistorial = {


    init: () => {

        $('#panelTabla').show();
        $('#frm')[0].reset();
        registroHistorial.SemanasAPrestar = 0;
        registroHistorial.idSeleccionado = -1;
        registroHistorial.accion = '';
        registroHistorial.fecha = '';
        registroHistorial.loadComboCustomerTypes();
        $('#table_').html('');

    },



    loadComboCustomerTypes: () => {

        var params = {};
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Customers/CustomerHistory.aspx/GetItemsCustomerTypes",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '';
                opcion += `<option value = ''>Seleccione...</option > `;

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdTipoCliente}' > ${item.NombreTipoCliente}</option > `;

                }

                $('#comboSemanasPrestamo').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },

    nuevoHistorial: (numeroSemanas) => {

        console.log('nuevoHistorial');

        registroHistorial.accion = "nuevo";
        registroHistorial.idEquipoSeleccionado = -1;


        $('#tableSolicitudes tbody').empty();


        let headers = '';
        headers += `<th scope="col"></th>`;

        let rows = [];
        rows[0] = `<th scope="col">A tiempo</th>`;
        rows[1] = `<th scope="col">Abonado</th>`;
        rows[2] = `<th scope="col">Falla</th>`;

        for (var i = 1; i <= numeroSemanas; i++) {
            headers += `<th scope="col text-center">${i}</th>`;
            rows[0] += `<th scope="col"><input type="radio" class="form-radio-control check-valores" id="colATiempo_${i}" name="col${i}"/></th>`;
            rows[1] += `<th scope="col"><input type="radio" class="form-radio-control check-valores" id="colAbonado_${i}" name="col${i}"/></th>`;
            rows[2] += `<th scope="col"><input type="radio" class="form-radio-control check-valores" id="colFalla_${i}" name="col${i}"/></th>`;
        }

        const htmlTable = `
                <table style="width: 100%!important;" class="table table-bordered table-hover table-striped text-center"
                    id="tableSolicitudes">

                    <thead class="thead-light">

                        ${headers}
                       
                    </thead>
                    <tbody>
                        <tr>
                        ${rows[0]}
                        </tr>
                        <tr>
                        ${rows[1]}
                        </tr>
                        <tr>
                        ${rows[2]}
                        </tr>
                    </tbody>
                </table>
        `;


        $('#table_').html(htmlTable);





    },






    accionesBotones: () => {




        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            solicitud.arrayData = [];
            registroHistorial.nuevasSolicitudes();



        });


        $('#comboSemanasPrestamo').on('change', (e) => {
            e.preventDefault();

            var params = {};
            params.path = "connbd";
            params.customerTypeId = $('#comboSemanasPrestamo').val();
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Customers/CustomerHistory.aspx/GetItemCustomerType",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    let item = msg.d;

                    console.log(`NumeroSemanas ${JSON.stringify(item.SemanasAPrestar)}`);

                    registroHistorial.nuevoHistorial(item.SemanasAPrestar);
                    registroHistorial.SemanasAPrestar = item.SemanasAPrestar;

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });



        $('#btnGuardar').on('click', (e) => {

            e.preventDefault();

            let hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (hasErrors) {

                $('#spnMensajes').html(mensajesAlertas.solicitudCamposVacios);
                $('#panelMensajes').modal('show');

                return;

            }


            let valoresTabla = [];
            // recolectar los datos
            const numRows = $('#tableSolicitudes tbody tr').length;

            for (let index = 0; index < registroHistorial.SemanasAPrestar; index++) {

                let col = index + 1;
                let newRow = {
                    ATiempo: $(`#colATiempo_${col}`).prop('checked') ? 1 : 0,
                    Abonado: $(`#colAbonado_${col}`).prop('checked') ? 1 : 0,
                    Falla: $(`#colFalla_${col}`).prop('checked') ? 1 : 0,

                };

                valoresTabla.push(newRow);

            }
            console.table(valoresTabla);

            let error = false;
            for (let i = 0; i < valoresTabla.length; i++) {
                let item = valoresTabla[i];

                //console.log(item);

                if (item.ATiempo === 0 && item.Abonado === 0 && item.Falla === 0) {
                    error = true;
                    break;
                }

            }

            if (error) {

                $('#spnMensajes').html(mensajesAlertas.solicitudCamposVaciosEnTablaHistorial);
                $('#panelMensajes').modal('show');
                return;
            }


            let parametros = {};
            parametros.path = "connbd";
            parametros.data = valoresTabla;
            parametros.curpCliente = $('#txtCurpCliente').val();
            parametros.curpAval = $('#txtCurpAval').val();
            parametros.idUsuario = document.getElementById('txtIdUsuario').value;
            parametros.idTipoCliente = $('#comboSemanasPrestamo').val();
            parametros.accion = 'guardar';
            parametros = JSON.stringify(parametros);


            $.ajax({
                type: "POST",                
                url: "../../pages/Customers/CustomerHistory.aspx/Save",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;



                    if (valores.CodigoError == 0) {

                        
                        $('#spnMensajes').html(mensajesAlertas.exitoGuardarHistorial);
                        $('#panelMensajes').modal('show');
                        registroHistorial.init();

                    } else {

                        $('#spnMensajes').html(valores.MensajeError);
                        $('#panelMensajes').modal('show');


                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);



                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });


        $('#btnAceptarPanelMensajes').on('click', (e) => {

            e.preventDefault();
            $('#panelMensajes').modal('hide');

            $('#panelTabla').show();
            $('#panelForm').hide();

        });


        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });


    }


}

window.addEventListener('load', () => {

    registroHistorial.init();

    registroHistorial.accionesBotones();

});



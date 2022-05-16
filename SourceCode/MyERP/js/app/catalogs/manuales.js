'use strict';
let date = new Date();
let descargas = "Manual_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '83';


const manuales = {


    init: () => {

    },



    accionesBotones: () => {



        $('.manuales').on('click', (e) => {
            e.preventDefault();

            //debugger;
            console.log(`btn_`);

            let urlManual = e.currentTarget.attributes['data-url'];
            console.log(`url = ${urlManual.value}`);

            let isMobile = utils.isMobile();
            if (!isMobile) {

                $(`#visor`).attr('src', urlManual.value);
            } else {
                window.open(urlManual.value);
            }

        });

    }


}

window.addEventListener('load', () => {

    manuales.init();

    manuales.accionesBotones();

});



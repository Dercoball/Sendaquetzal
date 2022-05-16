var getLocalization = function () {
    var localizationobj = {};
    localizationobj.pagergotopagestring = "Ir a:";
    localizationobj.pagershowrowsstring = "Muestra registros:";
    localizationobj.pagerrangestring = " de ";
    localizationobj.pagerpreviousbuttonstring = "Anterior";
    localizationobj.pagernextbuttonstring = "Siguiente";
    localizationobj.sortascendingstring = "Ascendente";
    localizationobj.sortdescendingstring = "Descendente";
    localizationobj.sortremovestring = "Remueve Orden";
    localizationobj.firstDay = 1;
    localizationobj.percentsymbol = "%";
    localizationobj.currencysymbol = "$";
    localizationobj.decimalseparator = ".";
    localizationobj.thousandsseparator = ",";
    localizationobj.filterclearstring = "Borrar Filtro";
    localizationobj.filterstring = "Filtro";
    localizationobj.filtershowrowstring = "Ver Registros = a";
    localizationobj.filterorconditionstring = "O";
    localizationobj.filterandconditionstring = "Y";
    localizationobj.filterstringcomparisonoperators = ['vacio', 'igual', 'es igual a', 'menor que', 'menor o igual', 'mayor que', 'mayor o igual', 'nulo', 'no nulo',
         'vacio', 'no está vacio', 'contiene', 'contiene (partido de caso)', 'no contiene', 'no contiene (partido de caso)', 'comienza con', 'comienza con (caso partido)',
         'termina con', 'termina con (caso partido)', 'igual', 'igual (caso partido)', 'nulo', 'no nullo'];
    localizationobj.filternumericcomparisonoperators = ['igual', 'es igual a', 'menor que', 'menor o igual', 'mayor que', 'mayor o igual', 'nulo', 'no nulo'];
    localizationobj.filterdatecomparisonoperators = ['igual', 'es igual a', 'menor que', 'menor o igual', 'mayor que', 'mayor o igual', 'nulo', 'no nulo'];

    var days = {
        names: ["domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado"],
        namesAbbr: ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"],
        namesShort: ["do", "lu", "ma", "mi", "ju", "vi", "sá"]
    };
    localizationobj.days = days;

    var months = {
        names: ["enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre", ""],
        namesAbbr: ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic", ""]
    };
    localizationobj.months = months;
    localizationobj.currencysymbol = "$";
    localizationobj.percentsymbol = "%";
    localizationobj.currencysymbolposition = "before";
    localizationobj.decimalseparator = ".";
    localizationobj.thousandsseparator = ",";
    localizationobj.loadtext = "Cargando...";
    localizationobj.emptydatastring = "No hay datos";
    //localizationobj.sum = "Suma";


    return localizationobj;
}
const VISTA_BUSQUEDA = {
    busquedaFecha: () => {
           //Acá se limpian las cajas de texto
        $("#txtFechaInicio").val("");
        $("#txtFechaFin").val("");
        $("#txtNumeroVenta").val("");

        $(".busqueda-fecha").show();
        $(".busqueda-venta").hide();
    },
    busquedaVenta: () => {
        //Acá se limpian las cajas de texto
        $("#txtFechaInicio").val("");
        $("#txtFechaFin").val("");
        $("#txtNumeroVenta").val("");

        $(".busqueda-fecha").hide();
        $(".busqueda-venta").show();
    }
}

$(document).ready(function () {
    VISTA_BUSQUEDA["busquedaFecha"]()

    $.datepicker.setDefaults($.datepicker.regional["es"])

    $("#txtFechaInicio").datepicker({dateFormat: "dd/mm/yy"})
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

})

$("#cboBuscarPor").change(function () {
    if ($("#cboBuscarPor").val() == "fecha") {
        VISTA_BUSQUEDA["busquedaFecha"]()
    } else {
        VISTA_BUSQUEDA["busquedaVenta"]()
    }
})

$("#btnBuscar").click(function () {
    if ($("#cboBuscarPor").val() == "fecha") {
        if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
            toastr.warning("", "Debe ingresar fecha inicio y fin")
            return;
        }
    } else {
        if ($("#txtNumeroVenta").val().trim() == "") {
            toastr.warning("", "Debe ingresar el numero de venta")
            return;
        }
    }

    // Obtener los valores de los campos de entrada
    const numeroVenta = $("#txtNumeroVenta").val();
    const fechaInicio = $("#txtFechaInicio").val();
    const fechaFin = $("#txtFechaFin").val();

    // Mostrar el overlay de carga
    const $cardBody = $(".card-body");
    $cardBody.find("div.row").LoadingOverlay("show");

    // Realizar la petición al servidor
    fetch(`/Venta/Historial?numeroVenta=${numeroVenta}&fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`)
        .then(response => {
            // Ocultar el overlay de carga
            $cardBody.find("div.row").LoadingOverlay("hide");

            // Verificar si la respuesta es satisfactoria
            if (response.ok) {
                return response.json();
            } else {
                throw new Error("Error en la respuesta del servidor");
            }
        })
        .then(responseJson => {
            // Limpiar la tabla de ventas
            const $tbVentaBody = $("#tbventa tbody");
            $tbVentaBody.empty();

            // Verificar si se encontraron ventas
            if (responseJson.length > 0) {
                // Agregar las ventas a la tabla
                responseJson.forEach(venta => {
                    const $btnVerVenta = $("<button>")
                        .addClass("btn btn-info btn-sm")
                        .append($("<i>").addClass("fas fa-eye"))
                        .data("venta", venta);

                    $tbVentaBody.append(
                        $("<tr>").append(
                            $("<td>").text(venta.fechaRegistro),
                            $("<td>").text(venta.numeroVenta),
                            $("<td>").text(venta.tipoDocumentoVenta),
                            $("<td>").text(venta.documentoCliente),
                            $("<td>").text(venta.nombreCliente),
                            $("<td>").text(venta.total),
                            $("<td>").append($btnVerVenta)
                        )
                    );
                });
            } else {
                // Mostrar un mensaje indicando que no se encontraron ventas
                $tbVentaBody.append(
                    $("<tr>").append(
                        $("<td>").attr("colspan", 7).text("No se encontraron ventas")
                    )
                );
            }
        })
        .catch(error => {
            // Ocultar el overlay de carga y mostrar un mensaje de error
            $cardBody.find("div.row").LoadingOverlay("hide");
            console.error(error);
            alert("Ocurrió un error al obtener el historial de ventas");
        });

})

$("#tbventa tbody").on("click", ".btn-info", function () {
    let d = $(this).data("venta");

    $("#txtFechaRegistro").val(d.fechaRegistro)
    $("#txtNumVenta").val(d.numeroVenta)
    $("#txtUsuarioRegistro").val(d.usuario)
    $("#txtTipoDocumento").val(d.tipoDocumentoVenta)
    $("#txtDocumentoCliente").val(d.documentoCliente)
    $("#txtNombreCliente").val(d.nombreCliente)
    $("#txtSubTotal").val(d.subTotal)
    $("#txtIVA").val(d.impuestoTotal)
    $("#txtTotal").val(d.total)


    // Limpiar la tabla de ventas
    const $tbProductos = $("#tbProductos tbody");
    $tbProductos.empty();

    d.detalleVenta.forEach((item) => {

        $tbProductos.append(
            $("<tr>").append(
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total),
            )
        );
    });

    $("#linkImprimir").attr("href",`MostrarPDFVenta?numeroVenta=${d.numeroVenta}`)
    $("#modalData").modal("show");
})
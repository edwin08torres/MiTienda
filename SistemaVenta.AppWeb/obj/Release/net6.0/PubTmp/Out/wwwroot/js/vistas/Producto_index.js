﻿const MODELO_BASE = {
    idProducto: 0,
    codigoBarra: "",
    marca: "",
    nombre: "",
    idCategoria: 0,
    stock: 0,
    urlImagen: "",
    precio: 0,
    esActivo: 1,
}

let tablaData;

$(document).ready(function () {

    fetch("/Categorias/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboCategoria").append(
                        $("<option>").val(item.idCategoria).text(item.descripcion)
                    )
                })
            }
        })

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Productos/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idProducto", "visible": false, "searchable": false },
            {
                "data": "urlImagen", render: function (data) {
                    return `<img style="height:60px" src=${data} class="rounded mx-auto d-block"/>`
                }
            },
            { "data": "codigoBarra" },
            { "data": "marca" },
            { "data": "descripcion" },
            { "data": "nombreCategoria" },
            { "data": "stock" },
            { "data": "precio" },
            {
                "data": "esActivo", render: function (data) {
                    if (data == 1)
                        return `<span class="badge badge-info">Activo</span>`;
                    else
                        return `<span class="badge badge-danger">No Activo</span>`;
                }
            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Productos',
                exportOptions: {
                    columns: [2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idProducto);
    $("#txtCodigoBarra").val(modelo.codigoBarra);
    $("#txtMarca").val(modelo.marca);
    $("#txtDescripcion").val(modelo.descripcion);
    $("#cboCategoria").val(modelo.idCategoria == 0 ? $("#cboCategoria option:first").val() : modelo.idCategoria)
    $("#txtStock").val(modelo.stock);
    $("#txtPrecio").val(modelo.precio);
    $("#cboEstado").val(modelo.esActivo);
    $("#txtImagen").val("");
    $("#imgProducto").attr("src", modelo.urlImagen);

    $("#modalData").modal("show");

}

$("#btnNuevo").click(function () {
    mostrarModal()
})

$("#btnGuardar").click(async function () {

    //debugger;

    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo : "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idProducto"] = parseInt($("#txtId").val())
    modelo["CodigoBarra"] = $("#txtCodigoBarra").val()
    modelo["marca"] = $("#txtMarca").val()
    modelo["Descripcion"] = $("#txtDescripcion").val()
    modelo["idCategoria"] = $("#cboCategoria").val()
    modelo["stock"] = $("#txtStock").val()
    modelo["precio"] = $("#txtPrecio").val()
    modelo["esActivo"] = $("#cboEstado").val()

    const inputFoto = document.getElementById("txtImagen");

    const formData = new FormData();

    formData.append("imagen", inputFoto.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    //if (modelo.idUsuario == 0) {
    //    var response = await axios.post("/Usuarios/Crear", formData);
    //    if (response.data.estado) {
    //        tablaData.row.add(response.data.objeto).draw(false);
    //        $("#modalData").modal("hide");
    //        toastr.success("El usuario fue creado");
    //    }
    //    else {
    //        toastr.error(response.data.mensaje);
    //    }
    //}
    //else {
    //    var response = await axios.put("/Usuarios/Editar", formData);
    //    if (response.data.estado) {
    //        tablaData.row(filaSelecionada).data(response.data.objeto).draw(false);
    //        filaSelecionada = null;
    //        $("#modalData").modal("hide");
    //        toastr.success("El usuario fue modificado");
    //    }
    //    else {
    //        toastr.error(response.data.mensaje);
    //    }
    //}
    //return Ok();

    if (modelo.idProducto == 0) {
        fetch("/Productos/Crear", {
            method: "POST",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo", "El producto fue creado", "success");
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error");

                }
            })
    } else {
        fetch("/Productos/Editar", {
            method: "PUT",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(filaSelecionada).data(responseJson.objeto).draw(false);
                    filaSelecionada = null;
                    $("#modalData").modal("hide");
                    swal("Listo", "El producto fue modificado", "success");
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error");

                }
            })
    }
})

let filaSelecionada;
$("#tbdata tbody").on("click", ".btn-editar", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSelecionada = $(this).closest("tr").prev();
    } else {
        filaSelecionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSelecionada).data();
    mostrarModal(data);
})

$("#tbdata tbody").on("click", ".btn-eliminar", function () {

    let fila;

    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    swal({
        title: "¿Está seguro?",
        text: `Eliminar el producto "${data.descripcion}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`/Productos/Eliminar?idProducto=${data.idProducto}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw();
                            swal("Listo", "El producto fue eliminado", "success");
                        } else {
                            swal("Lo sentimos", responseJson.mensaje, "error");

                        }
                    })
            }
        }
    )
})
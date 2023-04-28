const MODELO_BASE = {
    idCategoria: 0,
    descripcion: "",
    esActivo: 1
}

let tablaData;

$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Categorias/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idCategoria", "visible": false, "searchable": false },
            { "data": "descripcion" },
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
                filename: 'Reporte Categoria',
                exportOptions: {
                    columns: [1,2]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
}) 

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idCategoria);
    $("#txtDescripcion").val(modelo.descripcion);
    $("#cboEstado").val(modelo.esActivo);

    $("#modalData").modal("show");
}

$("#btnNuevo").click(function () {
    mostrarModal()
})

$("#btnGuardar").click(async function () {

    //debugger;

    if ($("#txtDescripcion").val().trim() == "" ) {
        toastr.warning("", "Debe completar el campo : descripcion")
        $("#txtDescripcion").focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idCategoria"] = parseInt($("#txtId").val())
    modelo["descripcion"] = $("#txtDescripcion").val()
    modelo["esActivo"] = $("#cboEstado").val()


    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idCategoria == 0) {
        fetch("/Categorias/Crear", {
            method: "POST",
            headers: {"content-Type":"application/json; charset=utf-8"},
            body: JSON.stringify(modelo)
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo", "La categoria fue creada", "success");
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error");

                }
            })
    } else {
        fetch("/Categorias/Editar", {
            method: "PUT",
            headers: { "content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo)
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
                    swal("Listo", "La categoria ha sido modificada", "success");
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
        text: `Eliminar la categoria "${data.descripcion}"`,
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

                fetch(`/Categorias/Eliminar?idCategoria=${data.idCategoria}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw();
                            swal("Listo", "La categoria fue eliminada", "success");
                        } else {
                            swal("Lo sentimos", responseJson.mensaje, "error");

                        }
                    })
            }
        }
    )
})
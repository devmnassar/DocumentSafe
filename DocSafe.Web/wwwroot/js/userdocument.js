var dataTable;

$(document).ready(function () {
    loadDataTable(); // Load table initially
});

function loadDataTable(searchQuery = '') {
    if ($.fn.DataTable.isDataTable('#tblData')) {
        dataTable.destroy(); // Destroy previous instance before reloading
    }

    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: searchQuery ? '/user/document/search' : '/user/document/getall',
            type: 'GET',
            data: { query: searchQuery },
            dataSrc: 'data'
        },
        "columns": [
            { data: 'id', "width": "10%" },
            { data: 'name', "width": "15%" },
            {
                data: 'added',
                "render": function (data) {
                    if (data) {
                        let date = new Date(data);
                        return date.toLocaleDateString('en-GB'); // Formats as "dd/mm/yyyy"
                    }
                    return ''; // Return empty if no date,
                },
                "width": "15%"
            },
            { data: 'note', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-100 btn-group">
                    <a href="/user/document/details/${data}" class="btn btn-primary btn-sm mx-2"><i class="bi bi-pencil-square"></i></a>
                     <a onClick=Delete('/user/document/delete/${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i>Delete</a>
                     <a href="/user/document/download/${data}" class="btn btn-secondary mx-2"> <i class="bi bi-trash-fill"></i>Download</a>
                    </div>`;
                },
                "width": "15%"
            }
        ]
    });
}

// Search button click event (Fixes the form submission issue)
$('#searchBtn').on('click', function (e) {
    e.preventDefault(); // Prevent default form behavior
    var searchValue = $('#searchInput').val();
    loadDataTable(searchValue); // Reload table with search results
});


function confirmDelete(id) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to recover this document!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            deleteDocument(id);
        }
    });
}

function deleteDocument(id) {
    $.ajax({
        url: '/user/document/delete/' + id,
        type: 'POST',
        success: function (response) {
            Swal.fire({
                title: "Deleted!",
                text: "Your document has been deleted.",
                icon: "success"
            }).then(() => {
                window.location.href = "/user/document/index"; // Redirect after delete
            });
        },
        error: function () {
            Swal.fire({
                title: "Error!",
                text: "Something went wrong while deleting.",
                icon: "error"
            });
        }
    });
}


function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    })
}
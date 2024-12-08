
$(document).ready(function () {
    $('#searchForm').submit(function (event) {
        event.preventDefault();

        var searchQuery = $('input[name="searchQuery"]').val(); // Get search query
        var statusFilter = $('#statusFilter').val(); // Get status filter
        var assigneeFilter = $('input[name="assigneeFilter"]').val(); // Get assignee filter
        var startDateFilter = $('input[name="startDateFilter"]').val(); // Get start date filter
        var endDateFilter = $('input[name="endDateFilter"]').val(); // Get end date filter

        $.ajax({
            url: '@Url.Action("Index", "ToDo")', // Use the same Index action for filtering
            type: 'GET',
            data: {
                searchQuery: searchQuery,
                statusFilter: statusFilter,
                assigneeFilter: assigneeFilter,
                startDateFilter: startDateFilter,
                endDateFilter: endDateFilter
            },
            success: function (response) {
                $('#taskListContainer').html(response); // Update task list or show no results
            },
            error: function (xhr, status, error) {
                console.error('Search Error:', error);
                Swal.fire('Error', 'An error occurred while searching. Please try again.', 'error');
            }
        });
    });
})





// Generalized notification function
function showNotification(title, message, icon, confirmButtonText = 'Okay') {
    Swal.fire({
        title: title,
        text: message,
        icon: icon,
        confirmButtonText: confirmButtonText
    });
}

// Toast-style notification function
function showToastNotification(title, icon = 'success') {
    Swal.fire({
        toast: true,
        position: 'top-end',
        icon: icon,
        title: title,
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true
    });
}

// AJAX-based task deletion with SweetAlert2 confirmation
$(document).ready(function () {
    function handleDelete(event) {
        event.preventDefault();

        var $deleteButton = $(event.target); // The clicked delete button
        var itemId = $deleteButton.data('id');

        // Validate ID
        if (!itemId || isNaN(itemId)) {
            showNotification('Invalid Task', 'Unable to identify the task to delete.', 'error');
            return;
        }

        // SweetAlert2 confirmation dialog
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                $deleteButton.prop('disabled', true);

                // Proceed with the AJAX request
                $.ajax({
                    url: '/Todo/DeleteConfirmed', // Ensure the correct URL
                    type: 'POST',
                    data: {
                        id: itemId,
                        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                    },
                    success: function (response) {
                        if (response.success) {
                            // Use toast notification for better UX
                            showToastNotification('Task deleted successfully!', 'success');
                            $deleteButton.closest('.card').fadeOut(500, function () {
                                $(this).remove();
                            });
                        } else {
                            showNotification('Error!', response.message, 'error');
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error('Delete Request Error:', { status, error, responseText: xhr.responseText });

                        showNotification('Error!', 'An unexpected error occurred. Please try again.', 'error');
                    },
                    complete: function () {
                        $deleteButton.prop('disabled', false);
                    }
                });
            }
        });
    }

    // Attach the delete event using event delegation
    $(document).on('click', '.delete-btn', handleDelete);
});
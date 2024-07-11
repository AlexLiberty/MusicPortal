function saveTabState() {
    const activeTab = document.querySelector('.nav-tabs .nav-link.active').getAttribute('id');
    localStorage.setItem('activeTab', activeTab);
}

function restoreTabState() {
    const activeTab = localStorage.getItem('activeTab');
    if (activeTab) {
        const tab = document.getElementById(activeTab);
        if (tab) {
            tab.click();
        }
    }
}

function confirmAction(url, userId, action) {
    saveTabState();

    Swal.fire({
        title: `Are you sure you want to ${action.toLowerCase()} this user?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#48e945',
        cancelButtonColor: '#e91212',
        customClass: {
            container: 'swal-container',
            popup: 'swal-popup',
            title: 'swal-title',
            confirmButton: 'confirm-btn',
            cancelButton: 'cancel-btn'
        },
        confirmButtonText: 'Yes, ' + action.toLowerCase() + ' it!'
    }).then((result) => {
        if (result.isConfirmed) {
            sendPostRequest(url, userId);
        }
    });
}

async function sendPostRequest(url, userId) {
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: new URLSearchParams({ 'userId': userId })
        });

        if (response.ok) {
            await updateTabContent();
        } else {
            Swal.fire('Error', 'There was an error processing your request.', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire('Error', 'There was an error processing your request.', 'error');
    }
}

async function updateTabContent() {
    try {
        const confirmationTab = document.querySelector('#confirmation');
        const managementTab = document.querySelector('#management');
        const genreTab = document.querySelector('#genre');

        const confirmationResponse = await fetch('/Admin/UserConfirmationPartial');
        const managementResponse = await fetch('/Admin/UserManagementPartial');
        const genreResponse = await fetch('/Admin/GenreManagementPartial');

        if (confirmationResponse.ok) {
            const confirmationHtml = await confirmationResponse.text();
            confirmationTab.innerHTML = confirmationHtml;
        }

        if (managementResponse.ok) {
            const managementHtml = await managementResponse.text();
            managementTab.innerHTML = managementHtml;
        }

        if (genreResponse.ok) {
            const genreHtml = await genreResponse.text();
            genreTab.innerHTML = genreHtml;
        }

        restoreTabState();
    } catch (error) {
        console.error('Error:', error);
    }
}

$(document).ready(function () {
    restoreTabState();

    $('#addGenreForm').on('submit', function (e) {
        e.preventDefault();

        saveTabState();

        var formData = $(this).serialize();

        $.ajax({
            type: 'POST',
            url: '/Admin/AddGenre', 
            data: formData,
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() 
            },
            success: function (response) {
          
                if (response && response.success) {
                    Swal.fire('Success', 'Genre added successfully!', 'success').then(async () => {
                        $('#addGenreModal').modal('hide');
                        await updateTabContent(); 
                    });
                } else {
                    Swal.fire('Error', response && response.message || 'An error occurred while adding the genre.', 'error');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
                Swal.fire('Error', 'An error occurred while adding the genre.', 'error');
            }
        });
    });

    $('#editGenreForm').on('submit', function (e) {
        e.preventDefault();
        $.ajax({
            type: 'POST',
            url: '/Admin/EditGenre',
            data: $(this).serialize(),
            success: function (response) {
                if (response && response.success) {
                    Swal.fire('Success', 'Genre updated successfully!', 'success').then(async () => {
                        $('#editGenreModal').modal('hide');
                        await updateTabContent();
                    });
                } else {
                    Swal.fire('Error', response && response.message || 'An error occurred while updating the genre.', 'error');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
                Swal.fire('Error', 'An error occurred while updating the genre.', 'error');
            }
        });
    });
});

function editGenre(id, name) {
    $('#editGenreId').val(id);
    $('#editGenreName').val(name);
    $('#editGenreModal').modal('show');
}

function deleteGenre(id) {
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
                type: 'POST',
                url: '/Admin/DeleteGenre',
                data: { id: id },
                success: function (response) {
                    if (response && response.success) {
                        Swal.fire('Deleted!', 'The genre has been deleted.', 'success').then(async () => {
                            await updateTabContent();
                        });
                    } else {
                        Swal.fire('Error', response && response.message || 'An error occurred while deleting the genre.', 'error');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error:', error);
                    Swal.fire('Error', 'An error occurred while deleting the genre.', 'error');
                }
            });
        }
    });
}

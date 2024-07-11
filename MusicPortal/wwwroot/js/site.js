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
        const tabSelectors = {
            confirmation: '#confirmation',
            management: '#management',
            genre: '#genre',
            music: '#music'
        };

        async function fetchHtml(url) {
            const response = await fetch(url);
            if (response.ok) {
                return await response.text();
            } else {
                console.error('Failed to fetch:', url);
                return '';
            }
        }

        const [confirmationHtml, managementHtml, genreHtml, musicHtml] = await Promise.all([
            fetchHtml('/Admin/UserConfirmationPartial'),
            fetchHtml('/Admin/UserManagementPartial'),
            fetchHtml('/Admin/GenreManagementPartial'),
            fetchHtml('/Admin/MusicManagementPartial')
        ]);

        document.querySelector(tabSelectors.confirmation).innerHTML = confirmationHtml;
        document.querySelector(tabSelectors.management).innerHTML = managementHtml;
        document.querySelector(tabSelectors.genre).innerHTML = genreHtml;
        document.querySelector(tabSelectors.music).innerHTML = musicHtml;

        restoreTabState();
    } catch (error) {
        console.error('Error:', error);
    }
}

function deleteGenre(id) {
    saveTabState();
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
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

/////////////////////////////////////////////////////////////////////////////////

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('submitGenreBtn').addEventListener('click', async function () {
        await sendFormRequest('addGenreForm', '/Admin/AddGenre');
    });
});

let isSubmitting = false;

async function sendFormRequest(formId, url) {
    if (isSubmitting) return;

    isSubmitting = true;

    try {

        const form = document.getElementById(formId);
        const formData = new FormData(form);

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        const response = await fetch(url, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': token
            }
        });

        if (response.ok) {
            const data = await response.json();
            if (data.success) {
                Swal.fire('Success', 'Genre added successfully!', 'success').then(async () => {
                    $('#addGenreModal').modal('hide');
                    await updateTabContent();
                    form.reset();
                });
            } else {
                Swal.fire('Error', data.message || 'An error occurred while adding the genre.', 'error');
            }
        } else {
            Swal.fire('Error', 'There was an error processing your request.', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire('Error', 'There was an error processing your request.', 'error');
    } finally {
        isSubmitting = false;
    }
}

$('#editGenreForm').on('submit', async function (e) {
    e.preventDefault();
    saveTabState();

    var formData = $(this).serialize();

    try {
        const response = await $.ajax({
            type: 'POST',
            url: '/Admin/EditGenre',
            data: formData,
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            dataType: 'json'
        });

        if (response && response.success) {
            await Swal.fire('Success', 'Genre updated successfully!', 'success');
            $('#editGenreModal').modal('hide');
            await updateTabContent();
            $('#editGenreForm')[0].reset();
        } else {
            await Swal.fire('Error', response.message || 'An error occurred while updating the genre.', 'error');
        }
    } catch (xhr) {
        console.error('Error:', xhr.statusText);
        const errorMessage = xhr.responseJSON?.message || 'An error occurred while updating the genre.';
        await Swal.fire('Error', errorMessage, 'error');
    }
});

function editGenre(id, name) {
    $('#editGenreId').val(id);
    $('#editGenreName').val(name);
    $('#editGenreModal').modal('show');
}


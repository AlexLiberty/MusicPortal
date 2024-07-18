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
            sendPostUserRequest(url, userId);
        }
    });
}

async function sendPostUserRequest(url, userId) {
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

        const confirmationHtml = await fetchHtml('/Admin/UserConfirmationPartial');
        const managementHtml = await fetchHtml('/Admin/UserManagementPartial');
        const genreHtml = await fetchHtml('/Admin/GenreManagementPartial');
        const musicHtml = await fetchHtml('/Admin/MusicManagementPartial');

        document.querySelector(tabSelectors.confirmation).innerHTML = confirmationHtml;
        document.querySelector(tabSelectors.management).innerHTML = managementHtml;
        document.querySelector(tabSelectors.genre).innerHTML = genreHtml;
        document.querySelector(tabSelectors.music).innerHTML = musicHtml;

        restoreTabState();
    } catch (error) {
        console.error('Error:', error);
    }
}

function deleteGenre(url, id) {
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
                url: url,
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

function deleteMusic(url, id) {
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
                url: url,
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

function editGenreModal(url, id, name) {
    document.getElementById('editGenreId').value = id;
    document.getElementById('editGenreName').value = name;
    $('#editGenreModal').modal('show');
}

function submitEditGenreForm() {
    var form = $('#editGenreForm');
    $.ajax({
        type: 'POST',
        url: form.attr('action'),
        data: form.serialize(),
        success: function(response) {
            if (response.success) {
                Swal.fire('Success!', 'Genre successfully edited.', 'success').then(() => {
                    $('#editGenreModal').modal('hide'); 
                    updateTabContent();
                });
            } else {
                Swal.fire('Error', response.message || 'An error occurred while editing the genre.', 'error');
            }
        },
        error: function(xhr, status, error) {
            console.error('Error:', error);
            Swal.fire('Error', 'An error occurred while editing the genre.', 'error');
        }
    });
}

function submitAddGenreForm() {
    var form = $('#addGenreForm')[0];
    var formData = new FormData(form);

    $.ajax({
        type: 'POST',
        url: $(form).attr('action'),
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                Swal.fire('Success!', 'Genre successfully added.', 'success').then(() => {
                    $('#addGenreModal').find('.close').click();
                    updateTabContent()
                        .then(() => {
                            console.log('Tab content updated successfully');
                        })
                        .catch(error => {
                            console.error('Error updating tab content:', error);
                        });
                });
            } else {
                Swal.fire('Error', response.message || 'An error occurred while adding the genre.', 'error');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            Swal.fire('Error', 'An error occurred while adding the genre.', 'error');
        }
    });
}
function submitAddMusicForm() {
    var form = $('#addMusicForm')[0];
    var formData = new FormData(form);

    $.ajax({
        type: 'POST',
        url: $(form).attr('action'),
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                Swal.fire('Success!', 'Music successfully added.', 'success').then(() => {
                    $('#addMusicModal').find('.close').click();
                    updateTabContent()
                        .then(() => {
                            console.log('Tab content updated successfully');
                        })
                        .catch(error => {
                            console.error('Error updating tab content:', error);
                        });
                });
            } else {
                Swal.fire('Error', response.message || 'An error occurred while adding the genre.', 'error');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            Swal.fire('Error', 'An error occurred while adding the genre.', 'error');
        }
    });
}

function editMusicModal(url, id, title, artist, genreId) {
    document.getElementById('editMusicId').value = id;
    document.getElementById('editMusicTitle').value = title;
    document.getElementById('editMusicArtist').value = artist;
    document.getElementById('editMusicGenre').value = genreId;
    $('#editMusicModal').modal('show');
}


function submitEditMusicForm() {
    var form = $('#editMusicForm')[0];
    var formData = new FormData(form);

    $.ajax({
        type: 'POST',
        url: $(form).attr('action'),
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                Swal.fire('Success!', 'Music successfully edited.', 'success').then(async () => {
                    $('#editMusicModal').modal('hide');
                    await updateTabContent();
                });
            } else {
                Swal.fire('Error', response.message || 'An error occurred while editing the music.', 'error');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            Swal.fire('Error', 'An error occurred while editing the music.', 'error');
        }
    });
}

function ideleteMusic(url, id) {
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
                url: url,
                data: { id: id },
                success: function (response) {
                    if (response && response.success) {
                        Swal.fire('Deleted!', 'The genre has been deleted.', 'success').then(async () => {
                            await updateContent();
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

$(document).ready(function () {
    $(".custom-close-btn").click(function () {
        var modal = $(this).closest(".modal");
        if (modal.length) {
            modal.modal('hide');
        }
    });
});

async function updateContent()
{
    try {
        const response = await $.ajax({
            url: 'Home/Index',
            type: 'POST',
            dataType: 'html'
        });

        $('table').html($(response).find('table').html());

    } catch (error) {
        console.error('Error updating content:', error);
        Swal.fire('Error', 'An error occurred while updating the content.', 'error');
    }
}

function playMusic(url) {
    $.getJSON(url, function (data) {
        if (data && data.url) {
            console.log("Setting music source:", data.url);
            $('#musicSource').attr('src', data.url);
            var audioPlayer = $('#musicPlayer')[0];
            audioPlayer.load();
            $('#playMusicModal').modal('show');

            $('#playMusicModal').on('shown.bs.modal', function () {
                console.log("Attempting to play music");
                audioPlayer.play().catch(function (error) {
                    console.error("Error playing music:", error);
                });
            });
        } else {
            console.error("Invalid response from server.");
        }
    }).fail(function () {
        console.error("Error fetching music URL.");
    });
}
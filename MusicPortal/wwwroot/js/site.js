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
        handleSortAndFilter();
        handleSortAndFilterMusic()
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
    saveTabState();
    var form = $('#editGenreForm');
    $.ajax({
        type: 'POST',
        url: form.attr('action'),
        data: form.serialize(),
        success: function (response) {
            if (response.success) {
                Swal.fire('Success!', 'Genre successfully edited.', 'success').then(async () => {
                    $('#editGenreModal').modal('hide');
                    await updateTabContent();
                });
            } else {
                Swal.fire('Error', response.message || 'An error occurred while editing the genre.', 'error');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            Swal.fire('Error', 'An error occurred while editing the genre.', 'error');
        }
    });
}

function submitAddGenreForm() {
    saveTabState();
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
                Swal.fire('Success!', 'Genre successfully added.', 'success').then(async () => {
                    $('#addGenreModal').find('.close').click();
                    await updateTabContent();
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
    saveTabState();
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
                Swal.fire('Success!', 'Music successfully added.', 'success').then(async () => {
                    $('#addMusicModal').find('.close').click();
                    await updateTabContent();
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
    saveTabState();
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

function submitEditMusicFormIndex() {
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
                    await updateContent();
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

function deleteUserMusic(url, id) {
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
                            await updateUserContent();
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

async function updateContent() {
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

async function updateUserContent() {
    try {
        const response = await $.ajax({
            url: 'User/Index',
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
            $('#playMusicModal').on('hidden.bs.modal', function () {
                console.log("Stopping music");
                audioPlayer.pause();
                audioPlayer.currentTime = 0;
            });
        } else {
            console.error("Invalid response from server.");
        }
    }).fail(function () {
        console.error("Error fetching music URL.");
    });
}

function submitAddUserMusicForm() {
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
                Swal.fire('Success!', 'Music successfully added.', 'success').then(async () => {
                    $('#addMusicModal').find('.close').click();
                    await updateUserContent();
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

function submitEditUserMusicForm() {
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
                    await updateUserContent();
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

//////////////////////////////////////////////////////////////////////////////
async function updateMusicTable(column, order, genreId, artist) {
    $.ajax({
        url: '/Admin/GetFilteredMusic',
        data: { column: column, order: order, genreId: genreId, artist: artist },
        success: function (data) {
            $('#musicTableBody').html(data);

            var newOrder = order === 'asc' ? 'desc' : 'asc';
            $('.sortable[data-column="' + column + '"]').data('order', newOrder);
        },
        error: function () {
            console.error("Error loading filtered music.");
        }
    });
}

async function handleSortAndFilterMusic() {
    $('.sortable').on('click', function () {
        var column = $(this).data('column');
        var order = $(this).data('order');
        var genreId = $('#genreFilter').val();
        var artist = $('#artistFilter').val();
        updateMusicTable(column, order, genreId, artist);
    });

    $('#genreFilter, #artistFilter').on('change keyup', function () {
        var column = $('.sortable[data-order]').data('column');
        var order = $('.sortable[data-order]').data('order');
        var genreId = $('#genreFilter').val();
        var artist = $('#artistFilter').val();
        updateMusicTable(column, order, genreId, artist);
    });
}

async function handleAction(url, userId, action) {
    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify({ userId }),
        contentType: 'application/json',
        success: async function () {
            await updateTabContent();
        },
        error: function () {
            console.error('Failed to perform action:', action);
        }
    });
}

///////////////////////////////////////////////////////////////////////////
async function updateUserTable(column, order, status, nameEmail) {
    try {
        const response = await fetch('/Admin/GetFilteredAndSortedUsers?' + $.param({ column, order, status, nameEmail }));
        if (response.ok) {
            const data = await response.text();
            $('#userTableBody').html(data);
            var newOrder = order === 'asc' ? 'desc' : 'asc';
            $('.sortable[data-column="' + column + '"]').data('order', newOrder);
        } else {
            console.error('Failed to fetch sorted and filtered users.');
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

function handleSortAndFilter() {
    $('.sortable').on('click', function () {
        var column = $(this).data('column');
        var order = $(this).data('order');
        var status = $('#statusFilter').val();
        var nameEmail = $('#nameEmailFilter').val();
        updateUserTable(column, order, status, nameEmail);
    });

    $('#statusFilter, #nameEmailFilter').on('change keyup', function () {
        var column = $('.sortable[data-order]').data('column');
        var order = $('.sortable[data-order]').data('order');
        var status = $('#statusFilter').val();
        var nameEmail = $('#nameEmailFilter').val();
        updateUserTable(column, order, status, nameEmail);
    });
}
$(document).ready(function () {
    $('.custom-close-btn').click(function () {
        var modal = $(this).closest(".modal");
        if (modal.length) {
            modal.modal('hide');
        }
    });
});

$(document).ready(function () {
    function updateMusicTableUser(column, order, genreId, artist) {
        $.ajax({
            url: '/User/GetFilteredMusic',
            data: { column: column, order: order, genreId: genreId, artist: artist },
            success: function (data) {
                $('#musicTableBody').html(data);
                var newOrder = order === 'asc' ? 'desc' : 'asc';
                $('.sortable[data-column="' + column + '"]').data('order', newOrder);
            },
            error: function () {
                console.log("Error loading filtered music.");
            }
        });
    }

    $('.sortable').on('click', function () {
        var column = $(this).data('column');
        var order = $(this).data('order');
        var genreId = $('#genreFilter').val();
        var artist = $('#artistFilter').val();
        updateMusicTableUser(column, order, genreId, artist);
    });

    $('#genreFilter, #artistFilter').on('change keyup', function () {
        var column = $('.sortable[data-order]').data('column');
        var order = $('.sortable[data-order]').data('order');
        var genreId = $('#genreFilter').val();
        var artist = $('#artistFilter').val();
        updateMusicTableUser(column, order, genreId, artist);
    });
});

$(document).ready(function ()
{
    $('#languageSelect').change(function ()
    {
        var selectedCulture = $(this).val();

        $.ajax(
            {
                type: 'POST',
                url: setLanguageUrl,
                data: { culture: selectedCulture },
                success: function (response)
                {
                    if (response.success)
                    {
                        location.reload();
                    }
                },
                error: function ()
                {
                    alert("ERROR CULTURE CHANGE.");
                }
            });
    });
});

//////////////////////////////////////////////////////////////////////

function updateFileName() {
    const fileInput = document.getElementById('musicFile');
    const fileName = fileInput.files.length > 0 ? fileInput.files[0].name : '@Localizer["No file selected"]';
    document.getElementById('fileName').value = fileName;
}
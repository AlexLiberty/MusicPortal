﻿function saveTabState() {
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





async function sendPostGenreRequest(url, userId) {
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
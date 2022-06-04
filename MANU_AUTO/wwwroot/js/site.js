// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Video play

document.addEventListener("DOMContentLoaded", () => {

    const input = document.getElementById('file-input');
    const video = document.getElementById('video');
    const videoSource = document.createElement('source');

    input.addEventListener('change', function () {
        const files = this.files || [];

        if (!files.length) return;

        const reader = new FileReader();

        reader.onload = function (e) {
            videoSource.setAttribute('src', e.target.result);
            video.appendChild(videoSource);
            video.load();
            video.play();
        };

        reader.onprogress = function (e) {
            console.log('progress: ', Math.round((e.loaded * 100) / e.total));
        };

        reader.readAsDataURL(files[0]);
    });
});
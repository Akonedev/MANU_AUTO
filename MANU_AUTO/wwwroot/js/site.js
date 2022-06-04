// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function visit(num) 

{
    if (sessionStorage.getItem(num) != null) {
        document.getElementById("vst").innerHTML = "page visitée";
        sessionStorage.setItem(num, 'visité');
    }
    else {
        return false;
    }
       
    //}
    //document.getElementById("demo").innerHTML = sessionStorage.num;
    console.log(sessionStorage.num);
}

function isVisited(nb) {
    if (visit(nb) != null) {
        document.getElementById("vst").innerHTML = "page visitée";
        document.getElementById("vst").style.display = "block";
    }
}
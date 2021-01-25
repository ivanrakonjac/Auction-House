// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function changeDisplay () {
    $("#image").css ( "display", "inline" ) ;
    $("#changeImageBtn").css ( "display", "none" ) ;
}

function readURL(input) {
    if (input.files && input.files[0]) {
      var reader = new FileReader();
      
      reader.onload = function(e) {
        $('#imagePreview').attr('src', e.target.result);
      }
      
      reader.readAsDataURL(input.files[0]); // convert to base64 string
    }
}
  
  $("#image").change(function() {
    readURL(this);
  });


function getSearchedAuctionsGET ( ) {

  var searchString = $("#searchString").val ();

  $.ajax ( {
    type: "GET",
    url: "/Auction/Search?searchString=" + searchString,
    dataType: "text",
    success: function ( response ){
      $("#pokusaj").html (response);
    },
    error: function ( response ){
        alert ( response );
    }
})

}

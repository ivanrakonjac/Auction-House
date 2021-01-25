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



function getSearchedAuctionsPages ( pageNumber ) {

  var searchString = $("#searchString").val ();
  var minPrice = $("#minPrice").val ();
  var maxPrice = $("#maxPrice").val ();
  var status = $("#selectedState").val ();

  $.ajax ( {
    type: "GET",
    url: "/Auction/Search?searchString=" + searchString + "&minPrice=" + minPrice + 
    "&maxPrice=" + maxPrice + "&status=" + status + "&pageNumber=" + pageNumber,
    dataType: "text",
    success: function ( response ){
      $("#searchedResult").html (response);
    },
    error: function ( response ){
        alert ( "response" );
    }
})

}

// Fukncija koja hvata enter u search boxu
$(".reagujNaEnter").on('keyup', function (e) {
  if (e.key === 'Enter' || e.keyCode === 13) {
      console.log ("ENTER");
      getSearchedAuctionsPages();
  }
});

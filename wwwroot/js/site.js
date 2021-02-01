// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Pravljenje objekta konekcije za slanje poruka serveru i za primanje notifikacija
var connection = new signalR.HubConnectionBuilder ( ).withUrl ( "/update" ).build ( );
connection.start ( );

function handleError ( error ) {
    alert ( error );
}

/* Fukncije koje se koriste da se odmah vidi slika po odabiru, a da ne ode u bazu */
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
/* Fukncije koje se koriste da se odmah vidi slika po odabiru, a da ne ode u bazu -- KRAJ*/


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


/**
 * Funkcija koja updeateuje vreme do kraja aukcije
 * @param {int} id - auctionId u bazi 
 */
function getTimeToEndOfAuction ( id ){

  var one_day = 1000*60*60*24;
  var one_hour = 1000*60*60;
  var one_minute = 1000*60;

  var seconds = $("#sekundCloseDate_" + id ).val ( );
  var minutes = $("#minutCloseDate_" + id ).val ( );
  var hours = $("#satCloseDate_" + id ).val ( );
  var day = $("#danCloseDate_" + id ).val ( );
  var month = $("#mesecCloseDate_" + id ).val ( );
  var year = $("#godinaCloseDate_" + id ).val ( );

  var closeDate = new Date(year, month - 1, day, hours, minutes, seconds, 0);
  var today = new Date();
  var timeToEnd = ( closeDate.getTime() - today.getTime() );

  if ( timeToEnd <= 0) {
    $('#bidButton_' + id ).prop('disabled', true);
    $("#timeToEndDiv_" + id ).removeClass('bg-danger');
    return;
  }

  var daysToEnd = timeToEnd / one_day;

  if ( daysToEnd > 1) {
    $("#dani_" + id ).text ( Math.round( daysToEnd ) + " days" );
    return;
  }

  var hoursToEnd = Math.trunc( timeToEnd / one_hour );

  if ( hoursToEnd >= 1) {
    $("#dani_" + id ).text ( "" );
    $("#sati_" + id ).text ( hoursToEnd + " h" );
    return;
  }

  $("#dani_" + id ).text ( "" );
  $("#sati_" + id ).text ( "" );

  var minutesToEnd = Math.trunc( timeToEnd / one_minute );

  if ( minutesToEnd >= 10) {
    $("#minuti_" + id ).text ( minutesToEnd + " min" );
    return;
  }

  var secondsToEnd = Math.trunc( (timeToEnd - minutesToEnd * one_minute ) / 1000 );

  if ( minutesToEnd >= 1) {
    $("#minuti_" + id ).text ( minutesToEnd + " min" );
  }else{
    $("#minuti_" + id ).text ( "" );
  }

  $("#sekunde_" + id ).text ( secondsToEnd + " s" );

  $("#timeToEndDiv_" + id ).addClass('bg-danger');


}

/**
 * Funkcija koja poziva fju getTimeToEndOfAuction za sve karitce
 */
function updateTimeToEndOfAuction ( ) {
  //Dohvatim sve elemente sa classom .auctionId i iz njih izvucem index aukcije u bazi (hidden filed)
  //Onda pozivam getTimeToEndOfAuction(auctionId)
  $('.auctionId').each(function(i, obj) {
    var index = i;
    var auctionId = $(this).val ( );
    getTimeToEndOfAuction ( auctionId );
  });
}

setInterval ( updateTimeToEndOfAuction, 1000 );


/**
 * Fukncija koja se poziva kada user pritisne BID na kartici proizvoda
 * @param {int} auctionID - id aukcije u bazi
 */
function bid ( auctionID ) {
  var rowVersion = $("#RowVersion_" + auctionID ).val ( );
  var verificationToken = $("input[name='__RequestVerificationToken']").val ( );

  $.ajax ( {
      type: "POST",
      dataType: "json",
      url: "/Auction/Bid",
      data: {
          "id" : auctionID,
          "rowVersion" : rowVersion,
          "__RequestVerificationToken" : verificationToken
      },
      success: function ( response ){
        console.log (response.rowVersion); 
        console.log (response.currentPrice); 

        $("#RowVersion_" + auctionID ).val ( response.rowVersion );
        $("#currentPrice_" + auctionID ).text ( "Cena: " +  response.currentPrice + " $" );
        
        connection.invoke ("NotifyUsers", auctionID);

      },
      error: function ( response ){
          alert ( response );
      }
  })
}

//setInterval ( bid, 10000, 0, 5 );

connection.on (
  "UpdateAuction",
  /**
   * funkcija koja se poziva pri promeni cene aukcije sa auctionID-jem na kartici sa id-jem
   * 
   * @param {int} auctionID - id aukcije u bazi
   */
  function (auctionID ) {

    var verificationToken = $("input[name='__RequestVerificationToken']").val ( );

    $.ajax ( {
      type: "POST",
      dataType: "json",
      url: "/Auction/getAuctionUpdateData",
      data: {
          "id" : auctionID,
          "__RequestVerificationToken" : verificationToken
      },
      success: function ( response ){
        console.log (response.rowVersion); 
        console.log (response.currentPrice); 

        $("#RowVersion_" + auctionID ).val ( response.rowVersion );
        $("#currentPrice_" + auctionID ).text ( "Cena: " +  response.currentPrice + " $" );

      },
      error: function ( response ){
          alert ( response );
      }
    })
  }
);
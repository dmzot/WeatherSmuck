const ws = new signalR.HubConnectionBuilder()
    .withUrl("/weather", { transport: signalR.HttpTransportType.WebSockets })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build();

let map;
let placemark;

function fetchError(err) {
    if (typeof err === 'string' || err instanceof String)
        var message = err;
    else
        message = err.message;
    $("#alert > p")[0].innerHTML = message;
    $("#alert").show();
}

function startWebSockets() {
    ws.start().catch(fetchError);
}

// Get coordinates from the address
var getCoordinates = function(address) {
    if (address.length > 0) {
        ymaps.geocode(address, { results: 1 }).then((res) => {
            // Choose the first result.
            let obj = res.geoObjects.get(0);
            // Coordinates of the object
            let coords = obj.geometry.getCoordinates();
            // Bounds of the object
            let bounds = obj.properties.get('boundedBy');
            
            if (placemark) {
                placemark.geometry.setCoordinates(coords);
            } else {
                placemark = obj;
                placemark.options.set('preset', 'islands#darkBlueDotIconWithCaption');
                map.geoObjects.add(placemark);
            }
            let addressLine = obj.getAddressLine();
            placemark.properties.set('iconCaption', addressLine);

            // Zoom the map
            map.setBounds(bounds, {
                checkZoomRange: true
            });

            $("#addr")[0].innerText = addressLine;
            getWeather(coords);
        });
    }
}

// Main function to get weather from WebSockets
var getWeather = function (coords) {
    let data = {
        lat: coords[0],
        lon: coords[1]
    }
    ws.invoke("GetCurrentWeather", data).then((res) => {
        if (res.errorMessage !== undefined && res.errorMessage !== null) {
            fetchError(res.errorMessage);
            return;
        }
        if (res.weather === undefined || res.weather.length === 0) {
            fetchError("Ooops. Smth went wrong. We'll fix it soon.");
            return;
        }
        let weather = JSON.parse(res.weather);
        setWeather(weather);
    }).catch(fetchError);
}

// Set all weather variables into html
var setWeather = function (w) {
    if (w.cod !== 200) {
        fetchError("Something went wrong with OpenWeatherAPI: " + w.message);
        return;
    }

    $("#weather_div").show();
    if (w.clouds.all !== undefined) {
        $("#dcloudness").show();
        $("#cloudness")[0].innerText = w.clouds.all + "%";
    }
    if (w.wind.speed !== undefined) {
        $("#dwind").show();
        $("#wind")[0].innerText = w.wind.speed + " m/sec";
    }
    if (w.main.temp !== undefined) {
        $("#dtemp").show();
        $("#temp")[0].innerText = "Now: " + w.main.temp + " C";
        $("#temp_max")[0].innerText = "Max: " + w.main.temp_max + " C";
        $("#temp_min")[0].innerText = "Min: " + w.main.temp_min + " C";
        $("#feels_like")[0].innerText = "Feels: " + w.main.feels_like + " C";
    }
    if (w.main.humidity !== undefined) {
        $("#dhumidity").show();
        $("#humidity")[0].innerText = w.main.humidity + "%";
    }
    if (w.weather.length) {
        let icon = w.weather[0].icon;
        $("#dicon").show();
        $("#icon").attr("class", "owi owi-" + icon);
    }
    console.log(w);
}


var addPlacemark = function(coords) {
    if (placemark) {
        placemark.geometry.setCoordinates(coords);
    }
    else {
        placemark = new ymaps.Placemark(coords,
            {
                iconCaption: 'searching...'
            },
            {
                preset: 'islands#darkBlueDotIconWithCaption',
                draggable: true
            });
        map.geoObjects.add(placemark);
        placemark.events.add('dragend', function () {
            getAddress(placemark.geometry.getCoordinates());
        });
    }
}

// Get address by coordinates
// See here: https://tech.yandex.ru/maps/jsbox/2.1/event_reverse_geocode
function getAddress(coords) {
    placemark.properties.set('iconCaption', 'поиск...');
    ymaps.geocode(coords).then(function (res) {
        let firstGeoObject = res.geoObjects.get(0);
        placemark.properties
            .set({
                iconCaption: [
                    firstGeoObject.getLocalities().length ? firstGeoObject.getLocalities() : firstGeoObject.getAdministrativeAreas(),
                    firstGeoObject.getThoroughfare() || firstGeoObject.getPremise()
                ].filter(Boolean).join(', '),
                balloonContent: firstGeoObject.getAddressLine()
            });
        $("#addr")[0].innerText = placemark.properties.get("iconCaption");
    });
}

ymaps.ready(() => {
    // Map initialization
    map = new ymaps.Map("map", {
        center: [55.76, 37.64], // Moscow
        zoom: 10,
        controls: ['zoomControl', 'fullscreenControl', 'geolocationControl']
    });

    map.events.add('click', function (e) {
        let coords = e.get('coords');
        addPlacemark(coords);
        getAddress(coords);
        getWeather(coords);
    });

    var suggestView = new ymaps.SuggestView('address',
        {
            results: 4,
            provider: {
                suggest: (function (request, options) {
                    return ymaps.suggest("Moscow, " + request); // TODO find Moscow bounds
                })
            }
        });
    suggestView.events.add('select', (event) => {
        var selected = event.get('item').value;
        getCoordinates(selected);
    });

    // If address string is not null (F5 reload in browser), start to get weather
    let value = $("#address").val();
    if (value.length > 0)
        getCoordinates(value);
});

$(document).ready(() => {
    // prevent default action on Enter keypress
    $('#address').on('keypress', function (e) {
        const k = e.keyCode || e.which;
        if (k === 13) {
            e.preventDefault();
            e.stopPropagation();
            getCoordinates($(this).val());
        }
    });
    $('#address').focus();
    startWebSockets();
})
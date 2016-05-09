var ViewModel = function () {
    var self = this;
    self.temperature = ko.observable('0');// ko.observableArray();
    self.error = ko.observable();

    var temperatureUri = 'api/temperature/';

    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        }).fail(function (jqXHR, textStatus, errorThrown) {
            self.error(errorThrown);
        });
    }


    this.UpdateTempCommand = function ()
    {
        ajaxHelper(temperatureUri + 'bysensor/0', 'GET')
            .done(function (data)
            {
                self.temperature(data.Temperature);
            });
    };
};

ko.applyBindings(new ViewModel());
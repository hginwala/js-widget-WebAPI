
var searchengine = new function () {
    this.init = function () {

        var format = 'json';
        var key = 'cc063147a7893807b8a7a4c51eb01246';
        var serviceurl = 'http://localhost:9810/api/values/';
        var recentjobsuri = serviceurl + 'RecentJobs/' + key;
        var featuredjobsuri = serviceurl + "search/" + key + "/"
            + "Software Developement Austin Texas" + "/" + "3";

        $(document).ready(function () {
            $('#job_input_text').autocomplete({
                minLength: 3,
                source: function (request, response) {
                    var keyword = $.trim($('#job_input_text').val());

                    var requesturl = serviceurl + "search" + "/" + key + "/" + keyword;

                    $.ajax(requesturl, { type: 'POST' }, { dataType: format })
                        .done(function (data) {
                            callback(data, '#jobs');
                        })
                        .fail(function (jqXHR, textStatus, err) {
                            failedcallback(jqXHR, textStatus, err, '#jobs');
                        });
                }
            });

            $.ajax( recentjobsuri,{ type: 'POST' }, { dataType: format })
                       .done(function (data) { callback(data, '#recentjobs'); })
                       .fail(function (jqXHR, textStatus, err) {
                           failedcallback(jqXHR, textStatus, err, '#recentjobs');
                       });

            $.ajax(featuredjobsuri, { type: 'POST' }, { dataType: format })
                      .done(function (data) { callback(data, '#featuredjobs'); })
                      .fail(function (jqXHR, textStatus, err) {
                          failedcallback(jqXHR, textStatus, err, '#featuredjobs');
                      });
        });

        function callback(data, selector) {
            if (format === 'json') {
                $(selector).html('');
                $.each(data, function (key, item) {
                    $('<li class="menuitem">')
                        .html("<a target='_blank' href='" + item.ApplyURL + "'>" + item.JobTitle + "</a>"
                                + "<span>" + item.City + ", " + item.State + "</span>")
                        .appendTo($(selector))
                });
            }
            else {
                $(selector).html('');
                $(data.children).find('JobDocument').each(function () {
                    var title = $(this).find('JobTitle').text();
                    var url = $(this).find('ApplyURL').text();
                    var city = $(this).find('City').text();
                    var state = $(this).find('State').text();
                    $('<li class="menuitem">')
                        .html("<a target='_blank' href='" + url + "'>" + title + "</a>"
                        + "<span>" + city + ", " + state + "</span>")
                        .appendTo($(selector))
                });
            }
        };

        function failedcallback(jqXHR, textStatus, err, selector) {
            $(selector).html("<span class='error'>" + jqXHR.responseJSON.Message + "</span>");
        }
    }
};
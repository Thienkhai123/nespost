(function () { angular.module('ui.select.pagination.groups', [])
/**
 * Provides with pagination with infinite scroll to handle large list of choices. 
 * An upfront large list of choices makes the control unstable and unresponsive.
 * This feature avoid populaing the list upfront by pagination which is the primary cause of unstability.
 * Pagination works in 2 scenarios:-
 * 
 * 1) Simple scrolling of the contents.
 * 2) Scrolling when the Autocomplete/search text is enteded and the results are still too large.
 * 
 * @example
    <ui-select-choices position="up" all-choices="ctrl.allTenThousandItems"  refresh-delay="0"
        repeat="person in $select.pageOptions.people | propsFilter: {name: $select.search, age: $select.search} ">
      <div ng-bind-html="person.name | highlight: $select.search"></div>
      <small>
        email: {{person.email}}
        age: <span ng-bind-html="''+person.age | highlight: $select.search"></span>
      </small>
    </ui-select-choices>
 * 
 * 
 */
.directive('uiSelectChoices', ['$timeout', '$parse', '$compile', '$document', '$filter', 
function ($timeout, $parse, $compile, $document, $filter) {
  return {
    link: function(scope, elm, attr) {
        var raw = elm[0];
        var scrollCompleted = true;
        if (!attr.allChoices) {
          // if there's no allChoices, then we can ignore this call, because it's
          // not a paginated select, just a regular one.
          // --DT
          return;

          // Original exception was thrown here:
          //throw new Error('ief:ui-select: Attribute all-choices is required in  ui-select-choices so that we can handle  pagination.');
        }

        // Patch to handle AJAX data loads, because Angular wasn't detecting a 
        // change in the model even when the built-in angular $http was used.
        // (This actually breaks when non-AJAX data is being used, so I'm
        // I'm commenting it out here, but it's needed for Product on Demand)

        /*scope.$watch(attr['allChoices'], function (newValue, oldValue, scope) {
          if (typeof newValue != 'undefined' && newValue['length'] > 0) {
            //noinspection JSUnresolvedFunction
            scope.pagingOptions = {
              allOptions: scope.$eval(attr['allChoices'])
            };
            scope.addMoreItems();
          }
        });*/

        scope.pagingOptions = {
          allOptions: scope.$eval(attr.allChoices)
        };

        attr.refresh = 'addMoreItems()';
        var refreshCallBack = $parse(attr.refresh);
        elm.bind('scroll', function(event) {
          var remainingHeight = raw.offsetHeight - raw.scrollHeight;
          var scrollTop = raw.scrollTop;
          var percent = Math.abs((scrollTop / remainingHeight) * 100);

          if (percent >= 80) {
            if (scrollCompleted) {
              scrollCompleted = false;
              event.preventDefault();
              event.stopPropagation();
              var callback = function() {
                scope.addingMore = true;
                refreshCallBack(scope, {
                  $event: event
                });
                scrollCompleted = true;

              };
              $timeout(callback, 100);
            }
          }
        });

        var closeDestroyer = scope.$on('uis:close', function() {
          var pagingOptions = scope.$select.pagingOptions || {};
          pagingOptions.filteredItems = undefined;
          pagingOptions.page = 0;
        });

        scope.addMoreItems = function(doneCalBack) {
          var $select = scope.$select;
          var allItems = scope.pagingOptions.allOptions;

          var moreItems = [];
          var itemsThreshold = 100;
          var search = $select.search;
        
            /* ADD LOGIC TO POPULATE groups using 
             * attr.groupBy in a meaningful way...
             */
            var groups = [ // Just a test... should be a function that parses longCityList
                {"name":"AK","items":[
                    {"key":"Adak Island-AK","display_name":"Adak Island, AK","group_by":"AK"},
                    {"key":"Anaktuvuk Pass-AK","display_name":"Anaktuvuk Pass, AK","group_by":"AK"},
                    {"key":"Anchorage-AK","display_name":"Anchorage, AK","group_by":"AK"}
                ]},
                {"name":"AL","items":[
                    {"key":"Alexander City-AL","display_name":"Alexander City, AL","group_by":"AL"},
                    {"key":"Aliceville-AL","display_name":"Aliceville, AL","group_by":"AL"},
                    {"key":"Andalusia-Opp-AL","display_name":"Andalusia-Opp, AL","group_by":"AL"},
                    {"key":"Anniston-AL","display_name":"Anniston, AL","group_by":"AL"},
                    {"key":"Ashland-Lineville-AL","display_name":"Ashland-Lineville, AL","group_by":"AL"},
                    {"key":"Atmore-AL","display_name":"Atmore, AL","group_by":"AL"},
                    {"key":"Auburn-AL","display_name":"Auburn, AL","group_by":"AL"},
                    {"key":"Bay Minette-AL","display_name":"Bay Minette, AL","group_by":"AL"},
                    {"key":"Bessemer-AL","display_name":"Bessemer, AL","group_by":"AL"}
                ]}
            ];
            
            /* THEN PAGINATE $select.groups IN A SIMILAR
             * WAY TO HOW pagingOptions PAGINATES $select.items
             */
            $select.groups = groups; // BUT PAGINATED

          var pagingOptions = $select.pagingOptions = $select.pagingOptions || {
            page: 0,
            pageSize: 20,
            items: $select.items
          };

          if (pagingOptions.page === 0) {
            pagingOptions.items.length = 0;
          }
          if (!pagingOptions.originalAllItems) {
            pagingOptions.originalAllItems = scope.pagingOptions.allOptions;
          }


          var searchDidNotChange = search && pagingOptions.prevSearch && search == pagingOptions.prevSearch;

          if (pagingOptions.filteredItems && searchDidNotChange) {
            allItems = pagingOptions.filteredItems;
          }
          pagingOptions.prevSearch = search;
          if (search && search.length > 0 && pagingOptions.items.length < allItems.length && !searchDidNotChange) {
            //search


            if (!pagingOptions.filteredItems) {
              //console.log('previous ' + pagingOptions.filteredItems);
            }

            pagingOptions.filteredItems = undefined;
            moreItems = $filter('filter')(pagingOptions.originalAllItems, search);
            //if filtered items are too many scrolling should occur for filtered items
            if (moreItems.length > itemsThreshold) {
              if (!pagingOptions.filteredItems) {
                pagingOptions.page = 0;
                pagingOptions.items.length = 0;
              } else {

              }
              pagingOptions.page = 0;
              pagingOptions.items.length = 0;
              allItems = pagingOptions.filteredItems = moreItems;

            } else {
              allItems = moreItems;
              pagingOptions.items.length = 0;
              pagingOptions.filteredItems = undefined;
            }


          }

          pagingOptions.page++;

          // Patch to do sanity checking on the data to make sure we HAVE data
          // before we try and process it
          // --DT
          if (typeof allItems != "undefined") {
            pagingOptions.page++;
            if (pagingOptions.page * pagingOptions.pageSize < allItems.length) {
              moreItems = allItems.slice(pagingOptions.items.length, pagingOptions.page * pagingOptions.pageSize);
            } else {
              moreItems = allItems;
            }
          }

          for (var k = 0; k < moreItems.length; k++) {
            pagingOptions.items.push(moreItems[k]);
          }

          scope.calculateDropdownPos();
          scope.$broadcast('uis:refresh');
          if (doneCalBack) doneCalBack();
        };
        scope.$on('$destroy', function() {
          elm.off('scroll');
          closeDestroyer();
        });
    }
  }
}])
/**
 * AngularJS default filter with the following expression:
 * "person in people | filter: {name: $select.search, age: $select.search}"
 * performs an AND between 'name: $select.search' and 'age: $select.search'.
 * We want to perform an OR.
 */
.filter('propsFilter', function() {
  return function(items, props) {
    var out = [];

    if (angular.isArray(items)) {
      var keys = Object.keys(props);

      items.forEach(function(item) {
        if (!item) return;
        var itemMatches = false;

        for (var i = 0; i < keys.length; i++) {
          var prop = keys[i];
          var text = props[prop].toLowerCase();
          if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
            itemMatches = true;
            break;
          }
        }

        if (itemMatches) {
          out.push(item);
        }
      });
    } else {
      // Let the output be the input untouched
      out = items;
    }

    return out;
  };
})

})();

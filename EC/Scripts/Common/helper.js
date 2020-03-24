String.prototype.replaceAll = function (search, replace) {
    return this.split(search).join(replace);
}

var cm = {
    defVal : function(val,def) {
        return typeof val !== 'undefined' ? val : def;
    },
    setInput: function(input, val) {
        input.val(val).change();
    }
}
var cmp = {

    OptionSelect: function(target, action) {
        var input = target.find('input[type="hidden"]');
        var options = target.find('.option');
        var other;
        if (target.find('input.other').length > 0) {
            other = target.find('input.other');
            other.change(function() {
                if (target.find('.option.active').length > 0)  cm.setInput(input,other.val());
            });
        }
        options.click(function () {
            var self = $(this);
            options.removeClass('active');
            self.addClass('active');
            cm.setInput(input, self.hasClass('other') ? self.find('input').val() : self.attr('data-val'));
            if (action != undefined) action(input.val());
        });
    },
    CustomSelect: function (target, withOthre) {
        withOthre =  cm.defVal(withOthre, false);
        target = target.wrapAll('<div class="selectRoot"></div>');
        target = target.closest('.selectRoot');
        function selectSetVal(val) {
            select.val(val);
            select.change();

        }
        var select = target.find('select');
        select.hide();
        var options = select.find('option');
       
        var builder = '<div class="customSelect"><input type="text" class="current"> <ul class="selectList">';
        options.each(function (i, val) {
            var selected = i == select[0].selectedIndex ? 'selected' : '';
            builder += '<li class="option" ' + selected
                       + ' data-val="' + val.value + '">' + val.innerText + '</option>';
        });
   
        builder += '</ul>';
        var root = $(builder);
        target.append(root);
        var currentInput = root.find('input.current');
        var optionsContainer = root.find('ul');
        currentInput.click(function() {
            optionsContainer.show();
        });
        if (withOthre) {
            currentInput.attr('readonly','readonly');
        }
        var newOptions = root.find('li.option');

        root.find('.option').click(function () {

            var self = $(this);
            newOptions.removeClass('active');
            self.addClass('active');
            selectSetVal(self.hasClass('other') ? other.val() : self.attr('data-val'));
            cm.setInput(currentInput, self.text());
            optionsContainer.hide();
        })
    }
}

var tmpl = {
    render: function (temp, map) {
        for (var i in map) 
            temp = temp.replaceAll('['+i.toUpperCase()+']', map[i]);
        return $(temp);
    }
}

var vl = {
    validateCls: 'validate',
    errorCls: 'vlError',
    correctCls: 'vlCorrect',


    url : function (url) {
        //var re = new RegExp("/^(?:(?:https?|ftp):\/\/)(?:\S+(?::\S*)?@)?(?:(?!10(?:\.\d{1,3}){3})(?!127(?:\.\d{1,3}){3})(?!169\.254(?:\.\d{1,3}){2})(?!192\.168(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]+-?)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]+-?)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:\/[^\s]*)?$/i");
        //	return re.test(url);
        return true;
    },
    email : function (email) {
        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    },

    notEmpty : function (value) {
        return value.length > 0;
    },

    date: function (date) {
        
        var today = new Date();
        today = new Date(today).setDate(today.getDate() + 1);

        var maxDate = Date.parse(today);


        var valueEntered = Date.parse(date);

        if (isNaN(valueEntered)) {
            return false;
        }
        if (valueEntered > maxDate) {
            return false;
        }
        return true;
    },
    login: function (login) {
        var re = /^[\w\.@]{2,20}$/;
        return re.test(login);
    },

    password: function (password) {
        var re = /^[\w\.@]{5,50}$/;
        return re.test(password);
    },
    phone: function (phone) {

        var re = /^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$/;
        return re.test(phone);
    },
    setCorrect: function (item) {
        item.removeClass(vl.errorCls);
        item.addClass(vl.correctCls);
        var target = item.attr('data-vltarget');
        if (target) {
            $(target).removeClass(vl.errorCls);
            $(target).addClass(vl.correctCls);
        }

    },
    setError : function(item) {
        item.removeClass(vl.correctCls);
        item.addClass(vl.errorCls);
        var target = item.attr('data-vlTarget');
        if (target) {
            $(target).removeClass(vl.correctCls);
            $(target).addClass(vl.errorCls);
        }
    },
    init : function(root) {
        var targets = root.find('.'+vl.validateCls);
        targets.change(function() {
            var self = $(this);
            var func = vl[self.attr('data-validate')];
            if (func(self.val())) vl.setCorrect(self);
            else vl.setError(self);
        })
    },
    check: function (root) {
         return root.find('.'+vl.validateCls).length == root.find('.'+vl.validateCls+'.'+vl.correctCls).length;
    }, 
    container: function(root) {
        
    }
}





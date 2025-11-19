(function () {

    class App {

        constructor() {

        }

        build(panelId) {
            this.panel = $('#' + panelId);

            const self = this;
            this.panel.on('click', '[my-click]', function (e) {
                console.log('target', e.target);
                console.log('currentTarget', e.currentTarget);

                let $target = $(e.currentTarget);
                let f = self[$target.attr('my-click')];
                if (typeof f === 'function')
                    f.call(self, $target);
            });
        }

        add($btn) {
            const id = Date.now();
            const newRow = $(`
<tr data-id="${id}">
    <td>${id}</td>
    <td>New Item</td>
    <td>dynamic add</td>
    <td>
        <button my-click="edit"><img src="images/edit.png">edit</button><button my-click="remove"><img src="images/remove.png">remove</button>
    </td>
</tr>`);

            this.panel.find('tbody').append(newRow);
        }

        edit($btn) {
            const id = $btn.closest('tr').data('id');
            alert(`edit id:${id}`);
        }

        remove($btn) {
            $btn.closest('tr').remove();
        }
    }

    const app = new App();
    document.addEventListener('DOMContentLoaded', () => {
        app.build('panel');
    });

})();
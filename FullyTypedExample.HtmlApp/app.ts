import domready = require('domready');
import { Repository } from './repository';

class Greeter {
    private element: HTMLElement;
    private table: HTMLElement;
    private repository: Repository;

    constructor(element: HTMLElement) {
        this.element = element;
        this.table = document.createElement('table');
        this.element.appendChild(this.table);
        this.repository = new Repository();
    }

    public makeRequest() {
        this.repository.getEmployees()
            .then((employees) => {
                // Generate html using tempalte string
                this.table.innerHTML = employees.reduce<string>((acc, x) => {
                        return `${acc}<tr><td>${x.id}</td><td>${x.name}</td></tr>`;
                    }, '');
            });
    }
}

domready(() => {
    var element = document.getElementById('content');
    var greeter = new Greeter(element);
    greeter.makeRequest();
});

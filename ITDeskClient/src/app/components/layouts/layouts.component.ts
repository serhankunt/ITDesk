import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { MenubarModule } from 'primeng/menubar';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-layouts',
  standalone: true,
  imports: [CommonModule,MenubarModule,RouterOutlet,InputTextModule,ButtonModule],
  templateUrl: './layouts.component.html',
  styleUrl: './layouts.component.css'
})
export class LayoutsComponent {
  items: MenuItem[] | undefined;

  constructor(private router:Router,public auth:AuthService){}

    ngOnInit() {
        this.items = [
            {
                label: 'Ana Sayfa',
                icon: 'pi pi-fw pi-home',
                routerLink:"/"
                // items: [
                //     {
                //         label: 'New',
                //         icon: 'pi pi-fw pi-plus',
                //         items: [
                //             {
                //                 label: 'Bookmark',
                //                 icon: 'pi pi-fw pi-bookmark'
                //             },
                //             {
                //                 label: 'Video',
                //                 icon: 'pi pi-fw pi-video'
                //             }
                //         ]
                //     },
                //     {
                //         label: 'Delete',
                //         icon: 'pi pi-fw pi-trash'
                //     },
                //     {
                //         separator: true
                //     },
                //     {
                //         label: 'Export',
                //         icon: 'pi pi-fw pi-external-link'
                //     }
                // ]
            }
        ];
    }
    logout(){
        localStorage.removeItem("response");
        location.href = "/login";
    }
}

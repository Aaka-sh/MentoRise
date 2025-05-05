import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { LikesService } from '../_services/likes.service';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { MemberCardComponent } from '../members/member-card/member-card.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-lists',
  imports: [ButtonsModule, FormsModule, MemberCardComponent, PaginationModule],
  templateUrl: './lists.component.html',
  styleUrl: './lists.component.css',
})
export class ListsComponent implements OnInit, OnDestroy {
  likeService = inject(LikesService); //injected likes service
  predicate = 'liked'; //default predicate for likes
  pageNumber = 1; //default page number
  pageSize = 5; //default page size

  ngOnInit(): void {
    this.loadLikes(); //load likes on initialization
  }

  getTitle() {
    switch (this.predicate) {
      case 'liked':
        return 'Members you added';
      case 'likedBy':
        return 'Members who added you';
      default:
        return 'Friends';
    }
  }

  loadLikes() {
    this.likeService.getLikes(this.predicate, this.pageNumber, this.pageSize); //get likes from service
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page; //update page number
      this.loadLikes(); //load likes again
    }
  }

  ngOnDestroy(): void {
    this.likeService.paginatedResult.set(null);
  }
}

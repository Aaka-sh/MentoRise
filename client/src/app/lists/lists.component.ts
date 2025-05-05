import { Component, inject, OnInit } from '@angular/core';
import { LikesService } from '../_services/likes.service';
import { Member } from '../_models/member';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { MemberCardComponent } from '../members/member-card/member-card.component';

@Component({
  selector: 'app-lists',
  imports: [ButtonsModule, FormsModule, MemberCardComponent],
  templateUrl: './lists.component.html',
  styleUrl: './lists.component.css',
})
export class ListsComponent implements OnInit {
  private likeService = inject(LikesService); //injected likes service
  members: Member[] = []; //importing member type from models
  predicate = 'liked'; //default predicate for likes

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
    this.likeService.getLikes(this.predicate).subscribe({
      next: (members) => {
        this.members = members;
      }, //set members to the response from the server
    });
  }
}
